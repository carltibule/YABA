using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YABA.Models;
using YABA.Models.Interfaces;

namespace YABA.Data.Context
{
    public class YABABaseContext : DbContext
    {
        public YABABaseContext(DbContextOptions<YABABaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add lookup backed data here
            // SAMPLE
            // var lookupBackedData = Enum.GetValues(typeof(LookupEnum)).Cast<LookupEnum>();
            // modelBuilder.Entity<LookupModel>().HasData(lookupBackedData.Select(x => new LookupModel(x)));


            modelBuilder.Model.GetEntityTypes()
                .Where(entityType => typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                .ToList()
                .ForEach(entityType =>
                {
                    modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(ConvertFilterExpression<ISoftDeletable>(e => !e.IsDeleted, entityType.ClrType));
                });

            modelBuilder.Entity<BookmarkTag>()
                .HasKey(x => new { x.BookmarkId, x.TagId });

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Auth0Id)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasIndex(x => x.Name)
                .IsUnique();
        }

        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BookmarkTag> BookmarkTags { get; set; }
        public DbSet<User> Users { get; set; }

        private static LambdaExpression ConvertFilterExpression<TInterface>(
                            Expression<Func<TInterface, bool>> filterExpression,
                            Type entityType)
        {
            // SOURCE: https://stackoverflow.com/questions/47673524/ef-core-soft-delete-with-shadow-properties-and-query-filters/48744644#48744644
            var newParam = Expression.Parameter(entityType);
            var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);

            return Expression.Lambda(newBody, newParam);
        }
    }
}
