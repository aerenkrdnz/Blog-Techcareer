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
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new ArticleTagConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Article> Articles => Set<Article>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<ArticleTag> ArticleTags => Set<ArticleTag>();
    }

}
