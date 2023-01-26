using System;
using System.Collections.Generic;
using System.Text;
using YABA.Service.DTO;

namespace YABA.Service.Interfaces
{
    public interface IUserService
    {
        public bool IsUserRegistered(string authProviderId);
        public UserDTO RegisterUser(string authProviderId);
    }
}
