using Blog.Data.Entities;
using Blog.Data.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class User : BaseEntity
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserTypeEnum UserType { get; set; }
    public string ProfileImageUrl { get; set; }
    public ICollection<Comment> Comments { get; set; }
}

public class UserConfiguration : BaseConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FirstName)
            .HasMaxLength(33);
        builder.Property(x => x.LastName)
            .HasMaxLength(33);
        builder.Property(x => x.Email)
            .HasMaxLength(52);
        builder.Property(x => x.ProfileImageUrl)
            .HasMaxLength(256);
        base.Configure(builder);
    }
}
