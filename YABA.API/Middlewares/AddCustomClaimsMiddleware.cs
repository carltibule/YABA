﻿using System.Security.Claims;
using YABA.API.Extensions;
using YABA.Common.Extensions;
using YABA.Common.Lookups;
using YABA.Service.Interfaces;

namespace YABA.API.Middlewares
{
    public class AddCustomClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public AddCustomClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IUserService userService)
        {
            if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
            {
                var userAuthProviderId = httpContext.User.Identity.GetAuthProviderId();

                if (!string.IsNullOrEmpty(userAuthProviderId))
                {
                    var userId = userService.GetUserId(userAuthProviderId);

                    if(userId <= 0)
                    {
                        var registedUser = await userService.RegisterUser(userAuthProviderId);
                        userId = registedUser.Id;
                    }

                    httpContext.User.Identities.FirstOrDefault().AddClaim(new Claim(ClaimsLookup.UserId.GetClaimName(), userId.ToString()));
                }
            }

            await _next(httpContext);
        }
    }
}
