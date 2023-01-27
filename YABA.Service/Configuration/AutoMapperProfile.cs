using AutoMapper;
using YABA.Common.DTOs;
using YABA.Models;

namespace YABA.Service.Configuration
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
        }
    }
}
