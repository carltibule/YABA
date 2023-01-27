﻿using AutoMapper;
using YABA.API.ViewModels;
using YABA.Common.DTOs;

namespace YABA.API.Settings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDTO, UserResponse>();
        }
    }
}
