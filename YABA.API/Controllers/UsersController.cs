using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YABA.API.Extensions;
using YABA.API.ViewModels;
using YABA.Service.Interfaces;

namespace YABA.API.Controllers
{
    [ApiVersion("1")]
    [Authorize, Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult Register()
        {
            var authProviderId = this.GetAuthProviderId();

            if (string.IsNullOrEmpty(authProviderId)) return NotFound();

            var isRegistered = _userService.IsUserRegistered(authProviderId);

            if (isRegistered) return NoContent();

            var registedUser = _userService.RegisterUser(authProviderId);
            return Ok(new UserResponse(registedUser));
        }
    }
}
