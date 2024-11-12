using CustomerPortal.Models.Enum;

namespace CustomerPortal.Models.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public ICollection<OrderItem> Items { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }
}
