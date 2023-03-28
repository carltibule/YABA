using AutoMapper;
using System.Net;
using YABA.API.ViewModels;
using YABA.API.ViewModels.Bookmarks;
using YABA.API.ViewModels.Tags;
using YABA.Common.DTOs;
using YABA.Common.DTOs.Bookmarks;
using YABA.Common.DTOs.Tags;

namespace YABA.API.Settings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDTO, UserResponse>();
            CreateMap<BookmarkDTO, BookmarkResponse>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => WebUtility.HtmlDecode(src.Title)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => WebUtility.HtmlDecode(src.Description)));
            CreateMap<WebsiteMetaDataDTO, GetWebsiteMetaDataResponse>();
            CreateMap<BookmarkDTO, PatchBookmarkRequest>();
            CreateMap<PatchBookmarkRequest, UpdateBookmarkRequestDTO>();
            CreateMap<TagDTO, TagResponse>().ReverseMap();
        }
    }
}
