namespace FitMe.Models.Configurations;

public class UserConfigurations:IEntityTypeConfiguration<ApplicationUser>
{
    

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {builder.OwnsMany(x=>x.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner
            ().HasForeignKey("UserId");
        builder.Property(x => x.Photo).HasMaxLength(1000).IsRequired(false);
        builder.Property(x => x.UserName).HasDefaultValue("null");
    }
}
