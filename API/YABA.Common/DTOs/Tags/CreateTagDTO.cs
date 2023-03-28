using System.ComponentModel.DataAnnotations;

namespace YABA.Common.DTOs.Tags
{
    public class CreateTagDTO
    {
        [Required]
        public string Name { get; set; }
        public bool IsHidden { get; set; }
    }
}
