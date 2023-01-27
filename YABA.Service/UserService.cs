using AutoMapper;
using System.Linq;
using YABA.Common.DTOs;
using YABA.Data.Context;
using YABA.Models;
using YABA.Service.Interfaces;

namespace YABA.Service
{
    public class UserService : IUserService
    {
        private readonly YABAReadOnlyContext _roContext;
        private readonly YABAReadWriteContext _context;
        private readonly IMapper _mapper;

        public UserService (YABAReadOnlyContext roContext, YABAReadWriteContext context, IMapper mapper)
        {
            _roContext = roContext;
            _context = context;
            _mapper = mapper;
        }

        public bool IsUserRegistered(string authProviderId)
        {
            return _roContext.Users.Any(x => x.Auth0Id == authProviderId);
        }

        public UserDTO RegisterUser(string authProviderId)
        {
            if(IsUserRegistered(authProviderId))
            {
                var user = _roContext.Users.FirstOrDefault(x => x.Auth0Id == authProviderId);
                return _mapper.Map<UserDTO>(user);
            }

            var userToRegister = new User
            {
                Auth0Id = authProviderId
            };

            var registedUser = _context.Users.Add(userToRegister);
            return _context.SaveChanges() > 0 ? _mapper.Map<UserDTO>(registedUser.Entity) : null;
        }

        public int GetUserId(string authProviderId)
        {
            var user = _roContext.Users.FirstOrDefault(x => x.Auth0Id == authProviderId);
            return user != null ? user.Id : 0;
        }
    }
}
