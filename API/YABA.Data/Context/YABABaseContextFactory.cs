
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace YABA.Data.Context
{
    public class YABABaseContextFactory : IDesignTimeDbContextFactory<YABABaseContext>
    {
        public YABABaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<YABABaseContext>();
            optionsBuilder.UseNpgsql(args[0])
                .UseSnakeCaseNamingConvention()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            return new YABABaseContext(optionsBuilder.Options);
        }
    }
}
