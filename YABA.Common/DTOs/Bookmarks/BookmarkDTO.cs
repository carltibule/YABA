using System;
using System.Collections.Generic;
using YABA.Common.DTOs.Tags;
using YABA.Common.Interfaces;

namespace YABA.Common.DTOs.Bookmarks
{
    public class BookmarkDTO : IBookmark
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public bool IsHidden { get; set; }
        public string Url { get; set; }
        public List<TagDTO>? Tags { get; set; } = new List<TagDTO>();
    }
}
