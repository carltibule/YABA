using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YABA.Service.Interfaces;

namespace YABA.Service.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddServiceProjectDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
        }
    }
}
