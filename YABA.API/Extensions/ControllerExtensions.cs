using Microsoft.AspNetCore.Mvc;
using YABA.Common.Lookups;

namespace YABA.API.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetAuthProviderId(this ControllerBase controller)
        {
            return controller.User.Identity.GetCustomClaim(ClaimsLookup.AuthProviderId);
        }

        public static int GetUserId(this ControllerBase controller)
        {
            var isValidUserId = int.TryParse(controller.User.Identity.GetCustomClaim(ClaimsLookup.UserId), out int userId);
            return isValidUserId ? userId : 0;
        }

        public static string GetIpAddress(this ControllerBase controller)
        {
            if (controller.Request.Headers.ContainsKey("X-Forwarded-For"))
                return controller.Request.Headers["X-Forwarded-For"];

            return controller.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
