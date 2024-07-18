using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        //Relational Property
        public ICollection<ArticleTag> ArticleTags { get; set; }
    }

    public class TagConfiguration : BaseConfiguration<Tag>
    {
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);
            base.Configure(builder);
        }
    }
}
