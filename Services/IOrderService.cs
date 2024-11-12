using CustomerPortal.Models.Common;
using CustomerPortal.Models.DTO;
using CustomerPortal.Models.Enum;
using CustomerPortal.Models.Response;

namespace CustomerPortal.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(Guid customerId, CreateOrderRequest request);
        Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId, Guid customerId);
        Task<IEnumerable<OrderResponseDto>> GetCustomerOrdersAsync(Guid customerId);
        Task<OrderResponseDto> CancelOrderAsync(Guid orderId, Guid customerId);
        Task<OrderResponseDto> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
        Task<PagedResponse<OrderResponseDto>> GetAllOrdersAsync(PaginationFilter filter);
        Task<PagedResponse<OrderResponseDto>> GetOrdersByStatusAsync(OrderStatus status, PaginationFilter filter);
    }
}
