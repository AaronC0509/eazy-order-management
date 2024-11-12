using System.Net;

namespace CustomerPortal.Models.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Address Address { get; set; }
        public ICollection<Order> Orders { get; set; }

        public Customer()
        {
            Orders = new List<Order>();
        }
    }
}
