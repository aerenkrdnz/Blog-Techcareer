using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Entities
{
    public class ArticleTag : BaseEntity
    {
        public int ArticleId { get; set; }
        public int TagId { get; set; }

        //Relational Property
        public Article Article { get; set; }
        public Tag Tag { get; set; }
    }

    public class ArticleTagConfiguration : BaseConfiguration<ArticleTag>
    {
        public override void Configure(EntityTypeBuilder<ArticleTag> builder)
        {
            builder.HasOne(at => at.Article)
                .WithMany(a => a.ArticleTags)
                .HasForeignKey(at => at.ArticleId);

            builder.HasOne(at => at.Tag)
                .WithMany(t => t.ArticleTags)
                .HasForeignKey(at => at.TagId);

            base.Configure(builder);
        }
    }

}
