using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YABA.Data.Context;

namespace YABA.Data.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDataProjectDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(x =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<YABABaseContext>();
                optionsBuilder.UseNpgsql(configuration.GetConnectionString("YABAReadOnlyDbConnectionString")).UseSnakeCaseNamingConvention();
                return new YABAReadOnlyContext(optionsBuilder.Options);
            });

            services.AddScoped(x => {
                var optionsBuilder = new DbContextOptionsBuilder<YABABaseContext>();
                optionsBuilder.UseNpgsql(configuration.GetConnectionString("YABAReadWriteDbConnectionString")).UseSnakeCaseNamingConvention();
                return new YABAReadWriteContext(optionsBuilder.Options);
            });

            services.AddDbContext<YABABaseContext>(options => options
                .UseNpgsql(configuration.GetConnectionString("YABAReadWriteDbConnectionString"))
                .UseSnakeCaseNamingConvention()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));

            services.AddDbContext<YABAReadWriteContext>(options => options
                .UseNpgsql(configuration.GetConnectionString("YABAReadWriteDbConnectionString"))
                .UseSnakeCaseNamingConvention()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));

            services.AddDbContext<YABAReadOnlyContext>(options => options
                .UseNpgsql(configuration.GetConnectionString("YABAReadOnlyDbConnectionString"))
                .UseSnakeCaseNamingConvention()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        }
    }
}
