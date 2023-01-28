
namespace YABA.Common.Interfaces
{
    public interface IBookmark
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public bool IsHidden { get; set; }
    }
}
