using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YABA.Models.Interfaces;

namespace YABA.Models
{
    public class Tag : IIdentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHidden { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<BookmarkTag> TagBookmarks { get; set; }
    }
}
