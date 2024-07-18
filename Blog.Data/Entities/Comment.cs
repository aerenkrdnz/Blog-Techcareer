using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; }
        public int ArticleId { get; set; }
        public int UserId { get; set; }
        //Relational Property
        public Article Article { get; set; }
        public User User { get; set; }
    }

    public class CommentConfiguration : BaseConfiguration<Comment>
    {
        public override void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(x => x.Article)
                .WithMany(a => a.Comments)
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);  

            builder.HasOne(x => x.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);  

            base.Configure(builder);
        }
    }

}
