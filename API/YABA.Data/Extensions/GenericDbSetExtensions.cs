using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using YABA.Models.Interfaces;

namespace YABA.Data.Extensions
{
    public static class GenericDbSetExtensions
    {
        public static void Upsert<T>(this DbSet<T> dbSet, T entity) where T : class, IIdentifiable
        {
            var entityInList = new List<T>() { entity };
            dbSet.UpsertRange(entityInList);
        }

        public static void UpsertRange<T>(this DbSet<T> dbSet, IEnumerable<T> entities) where T : class, IIdentifiable
        {
            foreach (var entity in entities)
            {
                var entityExists = dbSet.Any(x => x.Id == entity.Id);

                if (entityExists)
                {
                    EntityEntry<T> attachedEntity = dbSet.Attach(entity);
                    attachedEntity.State = EntityState.Modified;
                }
                else
                {
                    dbSet.Add(entity);
                }
            }
        }
    }
}
