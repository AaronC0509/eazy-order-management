using System.ComponentModel.DataAnnotations;

namespace CustomerPortal.Models.DTO
{
    public class CreateOrderRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<CreateOrderItemDto> Items { get; set; }
    }
}
