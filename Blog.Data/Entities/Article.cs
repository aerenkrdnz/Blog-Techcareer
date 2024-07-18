using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Entities
{
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public int UserId { get; set; }
        //Relational Property
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<ArticleTag> ArticleTags { get; set; }
    }

    public class ArticleConfiguration : BaseConfiguration<Article>
    {
        public override void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.Content)
                .IsRequired();
            builder.Property(x => x.ImageUrl)
                .HasMaxLength(256);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder.HasMany(x => x.ArticleTags)
                .WithOne(at => at.Article)
                .HasForeignKey(at => at.ArticleId);

            base.Configure(builder);
        }
    }

}