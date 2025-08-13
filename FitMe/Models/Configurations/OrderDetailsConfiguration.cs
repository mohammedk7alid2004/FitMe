namespace FitMe.Models.Configurations
{
    public class OrderDetailsConfiguration:IEntityTypeConfiguration<OrderDetail>

    {
       

        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.HasKey(od => od.OrderDetailId);
        
            builder.Property(od => od.Quantity)
                .IsRequired()
                .HasDefaultValue(1);
            builder.Property(od => od.UnitPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(od => od.TotalPrice)
                .HasColumnType("decimal(18,2")
                .HasComputedColumnSql("[Quantity] * [UnitPrice]");
            builder.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
