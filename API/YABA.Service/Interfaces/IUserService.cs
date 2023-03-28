using System.Threading.Tasks;
using YABA.Common.DTOs;

namespace YABA.Service.Interfaces
{
    public interface IUserService
    {
        bool IsUserRegistered(string authProviderId);
        Task<UserDTO> RegisterUser(string authProviderId);
        int GetUserId(string authProviderId);
    }
}
