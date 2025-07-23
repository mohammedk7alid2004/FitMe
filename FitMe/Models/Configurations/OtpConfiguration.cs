namespace FitMe.Models.Configurations
{
    public class OtpConfiguration: IEntityTypeConfiguration<OTP>
    {
        public void Configure(EntityTypeBuilder<OTP> builder)
        {
            builder.Property(x => x.Code)
        .IsRequired().HasMaxLength(6);


        }
    }
}
