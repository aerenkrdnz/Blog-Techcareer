using Microsoft.EntityFrameworkCore;
using Blog.Data.Entities;

namespace Blog.Data.Context
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ArticleConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Article> Articles => Set<Article>();
    }
}
