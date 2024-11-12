using CustomerPortal.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.PasswordHash)
                .IsRequired();

            builder.HasOne(c => c.Address)
                .WithOne(a => a.Customer)
                .HasForeignKey<Address>(a => a.CustomerId);

            builder.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId);
        }
    }
}
