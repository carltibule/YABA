using Microsoft.EntityFrameworkCore;

namespace YABA.Data.Context
{
    public class YABAReadOnlyContext : YABABaseContext
    {
        public YABAReadOnlyContext(DbContextOptions<YABABaseContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
    }
}
