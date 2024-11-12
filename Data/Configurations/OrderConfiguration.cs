using CustomerPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerPortal.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId);
        }
    }
}
