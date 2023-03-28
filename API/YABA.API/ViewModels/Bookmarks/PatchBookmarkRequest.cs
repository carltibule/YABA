namespace YABA.API.ViewModels.Bookmarks
{
    public class PatchBookmarkRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public bool IsHidden { get; set; }
        public string? Url { get; set; }
    }
}
