using CustomerPortal.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Street)
                .HasMaxLength(100);

            builder.Property(a => a.City)
                .HasMaxLength(50);

            builder.Property(a => a.State)
                .HasMaxLength(50);

            builder.Property(a => a.PostalCode)
                .HasMaxLength(20);

            builder.Property(a => a.Country)
                .HasMaxLength(50);
        }
    }
}
