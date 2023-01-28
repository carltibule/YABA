using AutoMapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YABA.Common.DTOs;
using YABA.Common.DTOs.Bookmarks;
using YABA.Common.DTOs.Tags;
using YABA.Common.Extensions;
using YABA.Common.Interfaces;
using YABA.Common.Lookups;
using YABA.Data.Context;
using YABA.Data.Extensions;
using YABA.Models;
using YABA.Service.Interfaces;

namespace YABA.Service
{
    public class BookmarkService : IBookmarkService
    {
        private readonly YABAReadOnlyContext _roContext;
        private readonly YABAReadWriteContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public BookmarkService(
            YABAReadOnlyContext roContext, 
            YABAReadWriteContext context, 
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _roContext = roContext;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public CrudResultDTO<IEnumerable<BookmarkDTO>> GetAll()
        {
            var currentUserId = GetCurrentUserId();

            var userBookmarks = _roContext.Bookmarks.Where(x => x.UserId == currentUserId).ToDictionary(k => k.Id, v => v);
            var bookmarkTagsLookup = _roContext.BookmarkTags
                    .Include(x => x.Tag)
                    .Where(x => userBookmarks.Keys.Contains(x.BookmarkId))
                    .ToList()
                    .GroupBy(x => x.BookmarkId)
                    .ToDictionary(k => k.Key, v => v.ToList());

            var userBookmarkDTOs = new List<BookmarkDTO>();

            foreach(var bookmark in userBookmarks)
            {
                var bookmarkDTO = _mapper.Map<BookmarkDTO>(bookmark.Value);
                bookmarkTagsLookup.TryGetValue(bookmark.Key, out var bookmarkTags);

                if(bookmarkTags != null)
                {
                    foreach (var bookmarkTag in bookmarkTags)
                    {
                        var tagDTO = _mapper.Map<TagSummaryDTO>(bookmarkTag.Tag);
                        bookmarkDTO.Tags.Add(tagDTO);
                    }
                }

                userBookmarkDTOs.Add(bookmarkDTO);
            }

            return new CrudResultDTO<IEnumerable<BookmarkDTO>> { Entry = userBookmarkDTOs, CrudResult = CrudResultLookup.RetrieveSuccessful };
        }

        public async Task<CrudResultDTO<CreateBookmarkRequestDTO>?> CreateBookmark(CreateBookmarkRequestDTO request)
        {
            var crudResult = new CrudResultDTO<CreateBookmarkRequestDTO>() { Entry = request, CrudResult = CrudResultLookup.CreateFailed };
            var currentUserId = GetCurrentUserId();

            if (!_roContext.Users.UserExists(currentUserId)) return crudResult;

            if(await _roContext.Bookmarks.AnyAsync(x => x.UserId == currentUserId && x.Url == request.Url))
            {
                crudResult.CrudResult = CrudResultLookup.CreateFailedEntryExists;
                return crudResult;
            }

            var bookmark = _mapper.Map<Bookmark>(request);
            UpdateBookmarkWithMetaData(bookmark);
            bookmark.UserId = currentUserId;

            await _context.Bookmarks.AddAsync(bookmark);

            if (await _context.SaveChangesAsync() > 0) crudResult.CrudResult = CrudResultLookup.CreateSucceeded;

            return crudResult;
        }

        public async Task<CrudResultDTO<UpdateBookmarkRequestDTO>> UpdateBookmark(int id, UpdateBookmarkRequestDTO request)
        {
            var crudResult = new CrudResultDTO<UpdateBookmarkRequestDTO>() { Entry = request, CrudResult = CrudResultLookup.UpdateFailed };
            var currentUserId = GetCurrentUserId();

            var bookmark = _context.Bookmarks.FirstOrDefault(x => x.UserId == currentUserId && x.Id == id);

            if(bookmark == null) return crudResult;

            bookmark.Title = request.Title;
            bookmark.Description = request.Description;
            bookmark.Note = request.Note;
            bookmark.IsHidden = request.IsHidden;
            bookmark.Url = request.Url;
            UpdateBookmarkWithMetaData(bookmark);

            if (await _context.SaveChangesAsync() > 0) crudResult.CrudResult = CrudResultLookup.UpdateSucceeded;

            return crudResult;
        }

        public async Task<IEnumerable<CrudResultDTO<string>>> UpdateBookmarkTags(int id, IEnumerable<string> tags)
        {
            var crudResults = tags.Select((x) => new CrudResultDTO<string> { Entry = x, CrudResult = CrudResultLookup.UpdateFailed }).ToList();
            var currentUserId = GetCurrentUserId();

            if (!_roContext.Bookmarks.Any(x => x.Id == id && x.UserId == currentUserId)) return crudResults;

            // Add tags that are not yet in the database
            var savedUserTags = _context.Tags.Where(x => x.UserId == currentUserId).ToList();
            var tagsToSave = tags.Except(savedUserTags.Select(x => x.Name).ToHashSet()).Select(x => new Tag { Name = x, UserId = currentUserId }).ToList();
            await _context.Tags.AddRangeAsync(tagsToSave);
            await _context.SaveChangesAsync();

            // Add newly added tags to the lookup
            savedUserTags.AddRange(tagsToSave);

            var existingBookmarkTags = _roContext.BookmarkTags.Include(x => x.Tag).Where(x => x.BookmarkId == id).ToList();
            var existingBookmarkTagsLookup = existingBookmarkTags.ToDictionary(k => k.TagId, v => v.Tag.Name);

            var bookmarkTagsToRemove = existingBookmarkTagsLookup
                .Where(x => !tags.Contains(x.Value))
                .Select(x => new BookmarkTag { BookmarkId = id, TagId = x.Key });

            var savedUserTagsByName = savedUserTags.ToDictionary(k => k.Name, v => v.Id);
            var bookmarkTagsToAdd = tags.Except(existingBookmarkTagsLookup.Values)
                .Select(x => new BookmarkTag { BookmarkId = id, TagId = savedUserTagsByName[x] });

            _context.BookmarkTags.RemoveRange(bookmarkTagsToRemove);
            await _context.BookmarkTags.AddRangeAsync(bookmarkTagsToAdd);

            if (await _context.SaveChangesAsync() >= 0) crudResults.ForEach(x => x.CrudResult = CrudResultLookup.UpdateSucceeded);

            return crudResults;
        }

        public async Task<CrudResultDTO<BookmarkDTO>> Get(int id)
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetUserId(), out int userId);
            var bookmark = await _roContext.Bookmarks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (bookmark == null) return new CrudResultDTO<BookmarkDTO> { CrudResult = CrudResultLookup.RetrieveFailed, Entry = null };

            var bookmarkTags = _roContext.BookmarkTags
                .Include(x => x.Tag)
                .Where(x => x.BookmarkId == id)
                .Select(x => x.Tag)
                .ToList();

            var bookmarkDTO = _mapper.Map<BookmarkDTO>(bookmark);
            bookmarkDTO.Tags = _mapper.Map<IList<TagSummaryDTO>>(bookmarkTags);

            return new CrudResultDTO<BookmarkDTO> { CrudResult = CrudResultLookup.RetrieveSuccessful, Entry = bookmarkDTO };
        }

        public CrudResultDTO<IEnumerable<TagSummaryDTO>> GetBookmarkTags(int id)
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetUserId(), out int userId);
            if (!_roContext.Bookmarks.Any(x => x.Id == id && x.UserId == userId)) return new CrudResultDTO<IEnumerable<TagSummaryDTO>> { Entry = null, CrudResult = CrudResultLookup.RetrieveFailed };

            var bookmarkTags = _roContext.BookmarkTags
                .Include(x => x.Tag)
                .Where(x => x.BookmarkId == id)
                .Select(x => x.Tag)
                .ToList();

            var bookmarkTagDTOs = _mapper.Map<IEnumerable<TagSummaryDTO>>(bookmarkTags);

            return new CrudResultDTO<IEnumerable<TagSummaryDTO>> { Entry = bookmarkTagDTOs, CrudResult = CrudResultLookup.RetrieveSuccessful };
        }

        public async Task<CrudResultDTO<int>> DeleteBookmark(int id)
        {
            var crudResults = await DeleteBookmarks(new List<int> { id });
            return crudResults.FirstOrDefault();
        }

        public async Task<IEnumerable<CrudResultDTO<int>>> DeleteBookmarks(IEnumerable<int> ids)
        {
            var crudResults = ids.Select(x => new CrudResultDTO<int> { Entry = x, CrudResult = CrudResultLookup.DeleteFailed }).ToList();
            var currentUserId = GetCurrentUserId();

            if (!await _roContext.Users.UserExistsAsync(currentUserId)) return crudResults;

            var entriesToDelete = _context.Bookmarks.Where(x => x.UserId == currentUserId && ids.Contains(x.Id)).ToList();
            var entryIdsToDelete = entriesToDelete.Select(x => x.Id);
            _context.Bookmarks.RemoveRange(entriesToDelete);

            if (await _context.SaveChangesAsync() <= 0) return crudResults;

            // Update crudResults that were found in the entriesToDelete to success
            foreach(var crudResult in crudResults)
            {
                if (entryIdsToDelete.Contains(crudResult.Entry))
                    crudResult.CrudResult = CrudResultLookup.DeleteSucceeded;
            }

            return crudResults;
        }

        private int GetCurrentUserId()
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetUserId(), out int userId);
            return userId;
        }

        private void UpdateBookmarkWithMetaData(IBookmark bookmark)
        {
            var webClient = new WebClient();
            var sourceData = webClient.DownloadString(bookmark.Url);
            var title = Regex.Match(sourceData, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
            var description = string.Empty;

            var getHtmlDoc = new HtmlWeb();
            var document = getHtmlDoc.Load(bookmark.Url);
            var metaTags = document.DocumentNode.SelectNodes("//meta");
            if (metaTags != null)
            {
                foreach (var sitetag in metaTags)
                {
                    if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value == "description")

                    {

                        description = sitetag.Attributes["content"].Value;
                    }
                }
            }

            bookmark.Title = !string.IsNullOrEmpty(bookmark.Title) ? bookmark.Title : string.IsNullOrEmpty(title) ? bookmark.Url : title;
            bookmark.Description = !string.IsNullOrEmpty(bookmark.Description) ? bookmark.Description : description;
        }
    }
}
