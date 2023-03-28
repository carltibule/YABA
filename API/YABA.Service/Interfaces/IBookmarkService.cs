using System.Collections.Generic;
using System.Threading.Tasks;
using YABA.Common.DTOs.Bookmarks;
using YABA.Common.DTOs.Tags;

namespace YABA.Service.Interfaces
{
    public interface IBookmarkService
    {
        Task<BookmarkDTO?> CreateBookmark(CreateBookmarkRequestDTO request);
        Task<BookmarkDTO?> UpdateBookmark(int id, UpdateBookmarkRequestDTO request);
        Task<IEnumerable<TagDTO>?> UpdateBookmarkTags(int id, IEnumerable<string> tags);
        IEnumerable<BookmarkDTO> GetAll(bool isHidden = false);
        Task<BookmarkDTO?> Get(int id);
        IEnumerable<TagDTO>? GetBookmarkTags(int id);
        Task<int?> DeleteBookmark(int id);
        Task<IEnumerable<int>?> DeleteBookmarks(IEnumerable<int> ids);
        Task<IEnumerable<int>?> HideBookmarks(IEnumerable<int> ids);
        IEnumerable<TagDTO> GetAllBookmarkTags(bool showHidden = false);
        
    }
}
