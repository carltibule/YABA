using System.ComponentModel.DataAnnotations;

namespace YABA.Common.DTOs.Tags
{
    public class UpdateTagDTO
    {
        public string? Name { get; set; }
        public bool? IsHidden { get; set; }
    }
}
