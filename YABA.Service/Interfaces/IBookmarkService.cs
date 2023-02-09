using System.Collections.Generic;
using System.Threading.Tasks;
using YABA.Common.DTOs;
using YABA.Common.DTOs.Bookmarks;
using YABA.Common.DTOs.Tags;

namespace YABA.Service.Interfaces
{
    public interface IBookmarkService
    {
        Task<BookmarkDTO?> CreateBookmark(CreateBookmarkRequestDTO request);
        Task<BookmarkDTO?> UpdateBookmark(int id, UpdateBookmarkRequestDTO request);
        Task<IEnumerable<TagSummaryDTO>?> UpdateBookmarkTags(int id, IEnumerable<string> tags);
        IEnumerable<BookmarkDTO> GetAll();
        Task<BookmarkDTO?> Get(int id);
        IEnumerable<TagSummaryDTO>? GetBookmarkTags(int id);
        Task<int?> DeleteBookmark(int id);
        Task<IEnumerable<int>?> DeleteBookmarks(IEnumerable<int> ids);
        
    }
}
