using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YABA.Common.Interfaces;
using YABA.Models.Interfaces;

namespace YABA.Models
{
    public class Bookmark : IIdentifiable, IDateCreatedTrackable, IDateModifiedTrackable, IBookmark
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public bool IsHidden { get; set; }
        public string Url { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public ICollection<BookmarkTag> BookmarkTags { get; set; }
    }
}
