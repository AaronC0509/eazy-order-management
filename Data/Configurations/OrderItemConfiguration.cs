using CustomerPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerPortal.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProductName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)");
        }
    }
}
