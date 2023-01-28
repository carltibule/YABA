using YABA.Common.DTOs;
using YABA.Common.Extensions;

namespace YABA.API.ViewModels
{
    public class GenericResponse<T>
    {
        public T Entry { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusMessage { get; set; }

        public GenericResponse(CrudResultDTO<T> value)
        {
            // TODO: Find a way to bring this to AutoMapper
            Entry = value.Entry;
            StatusId = (int)value.CrudResult;
            StatusName = value.CrudResult.ToString();
            StatusMessage = value.CrudResult.GetDisplayName();
        }
    }
}
