using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YABA.API.Extensions;
using YABA.API.ViewModels;
using YABA.Service.Interfaces;

namespace YABA.API.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UsersController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost]
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
            return Ok(_mapper.Map<UserResponse>(registedUser));
        }
    }
}
