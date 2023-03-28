using AutoMapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YABA.Common.DTOs.Bookmarks;
using YABA.Common.DTOs.Tags;
using YABA.Common.Extensions;
using YABA.Common.Interfaces;
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

        public IEnumerable<BookmarkDTO> GetAll(bool showHidden = false)
        {
            var currentUserId = GetCurrentUserId();
            var userBookmarkDTOs = new List<BookmarkDTO>();


            var showHiddenCondition = new Func<Bookmark, bool>(b => b.IsHidden || b.BookmarkTags.Where(x => x.Tag.UserId == currentUserId).Any(x => x.Tag.IsHidden));
            var showNotHiddenCondition = new Func<Bookmark, bool>(b => !b.IsHidden && b.BookmarkTags.Where(x => x.Tag.UserId == currentUserId).All(bt => !bt.Tag.IsHidden));

            var filteredBookmarks = _roContext.Bookmarks
                .Include(b => b.BookmarkTags)
                .ThenInclude(bt => bt.Tag)
                .Where(b => b.UserId == currentUserId)
                .Where(showHidden ? showHiddenCondition : showNotHiddenCondition)
                .ToList();

            foreach(var bookmark in filteredBookmarks)
            {
                var isBookmarkHidden = bookmark.IsHidden;
                var bookmarkDTO = _mapper.Map<BookmarkDTO>(bookmark);
                
                var bookmarkTags = bookmark.BookmarkTags.Select(x => x.Tag);
                bookmarkDTO.Tags = _mapper.Map<List<TagDTO>>(bookmarkTags);

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

            if (await _context.SaveChangesAsync() > 0)
            {
                var bookmarkDTO = _mapper.Map<BookmarkDTO>(newEntity.Entity);

                if(request.Tags != null && request.Tags.Any())
                    bookmarkDTO.Tags.AddRange(await UpdateBookmarkTags(bookmarkDTO.Id, request.Tags));
                return bookmarkDTO;
            }

            return null;
        }

        public async Task<BookmarkDTO?> UpdateBookmark(int id, UpdateBookmarkRequestDTO request)
        {
            var currentUserId = GetCurrentUserId();

            var bookmark = _context.Bookmarks.FirstOrDefault(x => x.UserId == currentUserId && x.Id == id);
            var tags = new List<TagDTO>();

            if (request.Tags != null && request.Tags.Any())
                tags = (await UpdateBookmarkTags(id, request.Tags)).ToList();


            if (bookmark == null) return null;

            bookmark.Title = !string.IsNullOrEmpty(request.Title) ? request.Title : bookmark.Title;
            bookmark.Description = !string.IsNullOrEmpty(request.Description) ? request.Description : bookmark.Description;
            bookmark.Note = !string.IsNullOrEmpty(request.Note) ? request.Note : bookmark.Note;
            bookmark.IsHidden = request.IsHidden;
            bookmark.Url = !string.IsNullOrEmpty(request.Url) ? request.Url : bookmark.Url;
            UpdateBookmarkWithMetaData(bookmark);

            await _context.SaveChangesAsync();
            var bookmarkDTO = _mapper.Map<BookmarkDTO>(bookmark);
            bookmarkDTO.Tags = tags;
            return bookmarkDTO;
        }

        public async Task<IEnumerable<TagDTO>?> UpdateBookmarkTags(int id, IEnumerable<string> tags)
        {
            var currentUserId = GetCurrentUserId();

            if (!_roContext.Bookmarks.Any(x => x.Id == id && x.UserId == currentUserId)
                 || tags == null || !tags.Any()) return null;

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

                return _mapper.Map<IEnumerable<TagDTO>>(updatedBookmarkTags);
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
            bookmarkDTO.Tags = _mapper.Map<List<TagDTO>>(bookmarkTags);

            return bookmarkDTO;
        }

        public IEnumerable<TagDTO>? GetBookmarkTags(int id)
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetUserId(), out int userId);
            if (!_roContext.Bookmarks.Any(x => x.Id == id && x.UserId == userId)) return null;

            var bookmarkTags = _roContext.BookmarkTags
                .Include(x => x.Tag)
                .Where(x => x.BookmarkId == id)
                .Select(x => x.Tag)
                .ToList();

            return _mapper.Map<IEnumerable<TagDTO>>(bookmarkTags);
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
            _context.Bookmarks.RemoveRange(entriesToDelete);

            if (await _context.SaveChangesAsync() <= 0) return null;

            return entriesToDelete.Select(x => x.Id);
        }

        public async Task<IEnumerable<int>?> HideBookmarks(IEnumerable<int> ids)
        {
            var currentUserId = GetCurrentUserId();

            if (!await _roContext.Users.UserExistsAsync(currentUserId)) return null;

            var entriesToHide = _context.Bookmarks.Where(x => x.UserId == currentUserId && ids.Contains(x.Id)).ToList();
            entriesToHide.ForEach((x) => { x.IsHidden = !x.IsHidden; });

            if(await _context.SaveChangesAsync() <= 0) return null;

            return entriesToHide.Select(x => x.Id);
        }

        public IEnumerable<TagDTO> GetAllBookmarkTags(bool showHidden = false)
        {
            var currentUserId = GetCurrentUserId();

            var activeUserTags = _roContext.BookmarkTags
                .Include(x => x.Tag)
                .Where(x => x.Tag.UserId == currentUserId && x.Tag.IsHidden == showHidden)
                .ToList()
                .GroupBy(x => x.Tag.Id)
                .Select(g => g.First()?.Tag);

            return _mapper.Map<IEnumerable<TagDTO>>(activeUserTags);
        }

        private int GetCurrentUserId()
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.Identity.GetUserId(), out int userId);
            return userId;
        }

        private void UpdateBookmarkWithMetaData(IBookmark bookmark)
        {
            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "APIClient");
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
