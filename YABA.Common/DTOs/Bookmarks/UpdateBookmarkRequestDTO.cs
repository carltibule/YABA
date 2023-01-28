using System.ComponentModel.DataAnnotations;
using YABA.Common.Interfaces;

namespace YABA.Common.DTOs.Bookmarks
{
    public class UpdateBookmarkRequestDTO : IBookmark
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public bool IsHidden { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
