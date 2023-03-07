using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YABA.Models.Interfaces;

namespace YABA.Data.Context
{
    public class YABAReadWriteContext : YABABaseContext
    {
        public YABAReadWriteContext() : base() { }

        public YABAReadWriteContext(DbContextOptions<YABABaseContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
        }

        public override int SaveChanges()
        {
            var dateCreatedTrackableEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IDateCreatedTrackable && e.State == EntityState.Added);

            foreach (var entry in dateCreatedTrackableEntries)
                ((IDateCreatedTrackable)entry.Entity).CreatedOn = DateTimeOffset.UtcNow;

            var dateModifiedTrackableItems = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IDateModifiedTrackable && (e.State == EntityState.Modified || e.State == EntityState.Added));

            foreach (var entry in dateModifiedTrackableItems)
                ((IDateModifiedTrackable)entry.Entity).LastModified = DateTimeOffset.UtcNow;

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var dateCreatedTrackableEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IDateCreatedTrackable && e.State == EntityState.Added);

            foreach (var entry in dateCreatedTrackableEntries)
                ((IDateCreatedTrackable)entry.Entity).CreatedOn = DateTimeOffset.UtcNow;

            var dateModifiedTrackableItems = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IDateModifiedTrackable && (e.State == EntityState.Modified || e.State == EntityState.Added));

            foreach (var entry in dateModifiedTrackableItems)
                ((IDateModifiedTrackable)entry.Entity).LastModified = DateTimeOffset.UtcNow;

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
