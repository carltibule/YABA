using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YABA.Common.Extensions;
using YABA.Common.Lookups;

namespace YABA.API.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetAuthProviderId(this ControllerBase controller)
        {
            return GetCustomClaim(controller, ClaimsLookup.AuthProviderId);
        }

        public static int GetUserId(this ControllerBase controller)
        {
            var isValidUserId = int.TryParse(GetCustomClaim(controller, ClaimsLookup.UserId), out int userId);
            return isValidUserId ? userId : 0;
        }

        public static string GetCustomClaim(this ControllerBase controller, ClaimsLookup claim)
        {
            var claimsIdentity = controller.User.Identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(claim.GetClaimName())?.Value.ToString();
        }

        public static string GetIpAddress(this ControllerBase controller)
        {
            if (controller.Request.Headers.ContainsKey("X-Forwarded-For"))
                return controller.Request.Headers["X-Forwarded-For"];

            return controller.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
