using System.Collections.Generic;
using System.Threading.Tasks;
using YABA.Common.DTOs;
using YABA.Common.DTOs.Bookmarks;
using YABA.Common.DTOs.Tags;

namespace YABA.Service.Interfaces
{
    public interface IBookmarkService
    {
        Task<CrudResultDTO<CreateBookmarkRequestDTO>> CreateBookmark(CreateBookmarkRequestDTO request);
        Task<CrudResultDTO<UpdateBookmarkRequestDTO>> UpdateBookmark(int id, UpdateBookmarkRequestDTO request);
        Task<IEnumerable<CrudResultDTO<string>>> UpdateBookmarkTags(int id, IEnumerable<string> tags);
        CrudResultDTO<IEnumerable<BookmarkDTO>> GetAll();
        Task<CrudResultDTO<BookmarkDTO>> Get(int id);
        CrudResultDTO<IEnumerable<TagSummaryDTO>> GetBookmarkTags(int id);
        Task<CrudResultDTO<int>> DeleteBookmark(int id);
        Task<IEnumerable<CrudResultDTO<int>>> DeleteBookmarks(IEnumerable<int> ids);
        
    }
}
