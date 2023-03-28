using Serilog.Context;
using YABA.Common.Extensions;

namespace YABA.API.Middlewares
{
    public class AddCustomLoggingPropertiesMiddleware
    {
        private readonly RequestDelegate _next;

        public AddCustomLoggingPropertiesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if(httpContext.Request.Path.HasValue && httpContext.Request.Path.Value.Contains("/api"))
            {
                LogContext.PushProperty("UserId", httpContext.User.Identity.IsAuthenticated ? httpContext.User.Identity.GetUserId() : "Anonymous");
                LogContext.PushProperty("RemoteIpAddress", httpContext.Connection.RemoteIpAddress.MapToIPv4());
            }

            await _next(httpContext);
        }
    }
}
