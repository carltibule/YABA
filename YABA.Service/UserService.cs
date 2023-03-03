using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly ILogger<UserService> _logger;

        public UserService (YABAReadOnlyContext roContext, YABAReadWriteContext context, IMapper mapper, ILogger<UserService> logger)
        {
            _roContext = roContext;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public bool IsUserRegistered(string authProviderId)
        {
            return _roContext.Users.Any(x => x.Auth0Id == authProviderId);
        }

        public async Task<UserDTO> RegisterUser(string authProviderId)
        {
            try
            {
                if (!IsUserRegistered(authProviderId))
                {
                    var userToRegister = new User
                    {
                        Auth0Id = authProviderId
                    };

                    var registedUser = _context.Users.Add(userToRegister);
                    await _context.SaveChangesAsync();
                }
            } 
            catch (Exception ex)
            {
                if(ex.InnerException is PostgresException &&
                    ((PostgresException)ex.InnerException).Code == "23505")
                {
                    var postgresException = (PostgresException)ex.InnerException;
                    _logger.LogWarning("Swallowing constraint violation: {@ConstraintName} for {@AuthProviderId}", postgresException.ConstraintName, authProviderId);
                }
                else
                {
                    throw ex;
                }
            }

            return await Get(authProviderId);
        }

        public async Task<UserDTO> Get(string authProviderId)
        {
            var user = await _roContext.Users.FirstOrDefaultAsync(x => x.Auth0Id == authProviderId);
            return _mapper.Map<UserDTO>(user);
        }

        public int GetUserId(string authProviderId)
        {
            var user = _roContext.Users.FirstOrDefault(x => x.Auth0Id == authProviderId);
            return user != null ? user.Id : 0;
        }
    }
}
