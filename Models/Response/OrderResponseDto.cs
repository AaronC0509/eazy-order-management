using CustomerPortal.Models.DTO;

namespace CustomerPortal.Models.Response
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
