using YABA.Common.Extensions;
using YABA.Common.Lookups;

namespace YABA.Common.DTOs
{
    public class CrudResultDTO<T>
    {
        public CrudResultLookup CrudResult { get; set; }
        public T Entry { get; set; }
        public bool IsSuccessful => CrudResult.IsCrudResultSuccessful();
    }
}
