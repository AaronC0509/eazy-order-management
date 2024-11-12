using System.ComponentModel.DataAnnotations;

namespace CustomerPortal.Models.DTO
{
    public class UpdateCustomerRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public AddressDto Address { get; set; }
    }
}
