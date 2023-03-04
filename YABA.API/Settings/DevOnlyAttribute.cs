using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace YABA.API.Settings
{
    // Source: https://stackoverflow.com/questions/56495475/asp-net-core-its-possible-to-configure-an-action-in-controller-only-in-developm
    public class DevOnlyAttribute : Attribute, IFilterFactory
    {
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new DevOnlyAttributeImpl(serviceProvider.GetRequiredService<IWebHostEnvironment>());
        }

        public bool IsReusable => true;

        private class DevOnlyAttributeImpl : Attribute, IAuthorizationFilter
        {
            public DevOnlyAttributeImpl(IWebHostEnvironment hostingEnv)
            {
                HostingEnv = hostingEnv;
            }

            private IWebHostEnvironment HostingEnv { get; }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (!HostingEnv.IsDevelopment())
                {
                    context.Result = new NotFoundResult();
                }
            }
        }
    }
}
