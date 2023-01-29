using System.Collections.Generic;
using YABA.Common.DTOs.Tags;

namespace YABA.Service.Interfaces
{
    public interface ITagsService
    {
        public IEnumerable<TagSummaryDTO> GetAll();
    }
}
