using YABA.API.ViewModels.Tags;

namespace YABA.API.ViewModels.Bookmarks
{
    public class BookmarkResponse
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public bool IsHidden { get; set; }
        public string Url { get; set; }
        public IEnumerable<TagResponse> Tags { get; set; }
    }
}
