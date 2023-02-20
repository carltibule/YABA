using System.Collections.Generic;
using System.Threading.Tasks;
using YABA.Common.DTOs.Tags;

namespace YABA.Service.Interfaces
{
    public interface ITagsService
    {
        Task<IEnumerable<TagDTO>?> GetAll();
        Task<TagDTO?> Get(int id);
        Task<IEnumerable<TagDTO>?> UpsertTags(IEnumerable<TagDTO> tags);
        Task<TagDTO?> UpsertTag(TagDTO tag);
        Task<IEnumerable<int>?> DeleteTags(IEnumerable<int> ids);
        Task<IEnumerable<int>?> HideTags(IEnumerable<int> ids);
        Task<TagDTO?> CreateTag(CreateTagDTO request);
        Task<TagDTO?> UpdateTag(int id, UpdateTagDTO request);
    }
}
