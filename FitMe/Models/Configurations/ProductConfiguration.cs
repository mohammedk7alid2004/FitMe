
using FitMe.Contracts.Product;

namespace FitMe.Models.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
   
        builder.HasOne(p => p.Category)
       .WithMany(c => c.Products) 
       .HasForeignKey(p => p.CategoryId)
       .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Brand)
               .WithMany(b => b.Products) 
               .HasForeignKey(p => p.BrandId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(x => x.Size)
            .HasMaxLength(10);
        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(1000);
        builder.Property(x => x.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.Property(x => x.Rating)
            
            .HasColumnType("decimal(3,2)");
        builder.Property(x => x.Stock)
            .IsRequired()
            .HasDefaultValue(0);

    }
}
