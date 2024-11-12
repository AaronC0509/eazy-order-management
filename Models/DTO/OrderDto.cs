namespace CustomerPortal.Models.DTO
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
