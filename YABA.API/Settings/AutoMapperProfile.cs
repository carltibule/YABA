﻿using AutoMapper;
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
            CreateMap<TagSummaryDTO, TagResponse>();
            CreateMap<BookmarkDTO, BookmarkResponse>();
            CreateMap<WebsiteMetaDataDTO, GetWebsiteMetaDataResponse>();
            CreateMap<BookmarkDTO, PatchBookmarkRequest>();
            CreateMap<PatchBookmarkRequest, UpdateBookmarkRequestDTO>();
        }
    }
}
