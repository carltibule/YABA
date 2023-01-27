using YABA.Common.DTOs;

namespace YABA.Service.Interfaces
{
    public interface IUserService
    {
        public bool IsUserRegistered(string authProviderId);
        public UserDTO RegisterUser(string authProviderId);
        public int GetUserId(string authProviderId);
    }
}
