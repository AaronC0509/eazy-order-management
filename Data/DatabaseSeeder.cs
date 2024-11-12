using CustomerPortal.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Customer>>();

            if (!await context.Customers.AnyAsync(c => c.Role == "Admin"))
            {
                var adminUser = new Customer
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Aaron",
                    LastName = "Chong",
                    Email = "jiazheng010509@gmail.com",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    PhoneNumber = "0143822321"
                };

                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");

                context.Customers.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
