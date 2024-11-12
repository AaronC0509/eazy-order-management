using CustomerPortal.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace CustomerPortal.Models.DTO
{
    public class UpdateOrderStatusRequest
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}
