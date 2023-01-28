using AutoMapper;
using YABA.Common.DTOs;
using YABA.Common.DTOs.Bookmarks;
using YABA.Common.DTOs.Tags;
using YABA.Models;

namespace YABA.Service.Configuration
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<BookmarkDTO, Bookmark>();
            CreateMap<Bookmark, BookmarkDTO>();
            CreateMap<CreateBookmarkRequestDTO, Bookmark>();
            CreateMap<Tag, TagDTO>();
            CreateMap<Tag, TagSummaryDTO>();
        }
    }
}
