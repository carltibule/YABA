using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YABA.Models
{
    public class BookmarkTag
    {
        [Required]
        [ForeignKey(nameof(Bookmark))]
        public int BookmarkId { get; set; }
        public virtual Bookmark Bookmark { get; set; }

        [Required]
        [ForeignKey(nameof(Tag))]
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
