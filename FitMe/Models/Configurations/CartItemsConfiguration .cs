

namespace FitMe.Models.Configurations;

public class CartItemsConfiguration : IEntityTypeConfiguration<CartItems>
{
  
    public void Configure(EntityTypeBuilder<CartItems> builder)
    {
      builder.HasOne(x=>x.Cart)
       .WithMany(Cart => Cart.Items)
       .HasForeignKey(x => x.CartId)
       .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x=>x.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasDefaultValue(1);
        builder.Property(x => x.AddedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();
   
    }
}
