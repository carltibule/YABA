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

        public IEnumerable<BookmarkDTO> GetAll()
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

            return userBookmarkDTOs;
        }

        public async Task<BookmarkDTO?> CreateBookmark(CreateBookmarkRequestDTO request)
        {
            var currentUserId = GetCurrentUserId();

            if (!_roContext.Users.UserExists(currentUserId)
                || await _roContext.Bookmarks.AnyAsync(x => x.UserId == currentUserId && x.Url == request.Url)) return null;

            var bookmark = _mapper.Map<Bookmark>(request);
            UpdateBookmarkWithMetaData(bookmark);
            bookmark.UserId = currentUserId;

            var newEntity = await _context.Bookmarks.AddAsync(bookmark);

            if (await _context.SaveChangesAsync() > 0) return _mapper.Map<BookmarkDTO>(newEntity.Entity);

            return null;
        }

        public async Task<BookmarkDTO?> UpdateBookmark(int id, UpdateBookmarkRequestDTO request)
        {
            var currentUserId = GetCurrentUserId();

            var bookmark = _context.Bookmarks.FirstOrDefault(x => x.UserId == currentUserId && x.Id == id);

            if(bookmark == null) return null;

            bookmark.Title = !string.IsNullOrEmpty(request.Title) ? request.Title : bookmark.Title;
            bookmark.Description = !string.IsNullOrEmpty(request.Description) ? request.Description : bookmark.Description;
            bookmark.Note = !string.IsNullOrEmpty(request.Note) ? request.Note : bookmark.Note;
            bookmark.IsHidden = request.IsHidden;
            bookmark.Url = !string.IsNullOrEmpty(request.Url) ? request.Url : bookmark.Url;
            UpdateBookmarkWithMetaData(bookmark);

            if (await _context.SaveChangesAsync() > 0) return _mapper.Map<BookmarkDTO>(bookmark);

            return null;
        }

        public async Task<IEnumerable<TagSummaryDTO>?> UpdateBookmarkTags(int id, IEnumerable<string> tags)
        {
            var currentUserId = GetCurrentUserId();

            if (!_roContext.Bookmarks.Any(x => x.Id == id && x.UserId == currentUserId)) return null;

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

            if (await _context.SaveChangesAsync() >= 0)
            {
                var updatedBookmarkTags = _roContext.BookmarkTags
                    .Include(x => x.Tag)
                    .Where(x => x.BookmarkId == id)
                    .Select(x => x.Tag);

                return _mapper.Map<IEnumerable<TagSummaryDTO>>(updatedBookmarkTags);
            }

            return null;
        }

        public async Task<BookmarkDTO?> Get(int id)
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetUserId(), out int userId);
            var bookmark = await _roContext.Bookmarks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (bookmark == null) return null;

            var bookmarkTags = _roContext.BookmarkTags
                .Include(x => x.Tag)
                .Where(x => x.BookmarkId == id)
                .Select(x => x.Tag)
                .ToList();

            var bookmarkDTO = _mapper.Map<BookmarkDTO>(bookmark);
            bookmarkDTO.Tags = _mapper.Map<IList<TagSummaryDTO>>(bookmarkTags);

            return bookmarkDTO;
        }

        public IEnumerable<TagSummaryDTO>? GetBookmarkTags(int id)
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetUserId(), out int userId);
            if (!_roContext.Bookmarks.Any(x => x.Id == id && x.UserId == userId)) return null;

            var bookmarkTags = _roContext.BookmarkTags
                .Include(x => x.Tag)
                .Where(x => x.BookmarkId == id)
                .Select(x => x.Tag)
                .ToList();

            return _mapper.Map<IEnumerable<TagSummaryDTO>>(bookmarkTags);
        }

        public async Task<int?> DeleteBookmark(int id)
        {
            var result = await DeleteBookmarks(new List<int> { id });
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<int>?> DeleteBookmarks(IEnumerable<int> ids)
        {
            var currentUserId = GetCurrentUserId();

            if (!await _roContext.Users.UserExistsAsync(currentUserId)) return null;

            var entriesToDelete = _context.Bookmarks.Where(x => x.UserId == currentUserId && ids.Contains(x.Id)).ToList();
            var entryIdsToDelete = entriesToDelete.Select(x => x.Id);
            _context.Bookmarks.RemoveRange(entriesToDelete);

            if (await _context.SaveChangesAsync() <= 0) return null;

            return ids;
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
