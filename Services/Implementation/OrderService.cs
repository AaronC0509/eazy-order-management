using AutoMapper;
using CustomerPortal.Data;
using CustomerPortal.Exceptions;
using CustomerPortal.Models.Common;
using CustomerPortal.Models.DTO;
using CustomerPortal.Models.Entities;
using CustomerPortal.Models.Enum;
using CustomerPortal.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(Guid customerId, CreateOrderRequest request)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
                throw new NotFoundException($"Customer with ID {customerId} not found");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                OrderNumber = GenerateOrderNumber(),
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                Items = request.Items.Select(item => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            order.TotalAmount = order.Items.Sum(item => item.Quantity * item.UnitPrice);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId, Guid customerId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new NotFoundException($"Order with ID {orderId} not found");

            if (order.CustomerId != customerId)
                throw new UnauthorizedAccessException("Access to this order is not allowed");

            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetCustomerOrdersAsync(Guid customerId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
        }

        public async Task<OrderResponseDto> CancelOrderAsync(Guid orderId, Guid customerId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new NotFoundException($"Order with ID {orderId} not found");

            if (order.CustomerId != customerId)
                throw new UnauthorizedAccessException("Access to this order is not allowed");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be cancelled");

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<OrderResponseDto> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new NotFoundException($"Order with ID {orderId} not found");

            if (!IsValidStatusTransition(order.Status, newStatus))
                throw new InvalidOperationException($"Invalid status transition from {order.Status} to {newStatus}");

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<PagedResponse<OrderResponseDto>> GetAllOrdersAsync(PaginationFilter filter)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking();

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<OrderResponseDto>>(items);

            return new PagedResponse<OrderResponseDto>(dtos, filter.PageNumber, filter.PageSize, totalItems);
        }

        public async Task<PagedResponse<OrderResponseDto>> GetOrdersByStatusAsync(OrderStatus status, PaginationFilter filter)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking();

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<OrderResponseDto>>(items);

            return new PagedResponse<OrderResponseDto>(dtos, filter.PageNumber, filter.PageSize, totalItems);
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (OrderStatus.Pending, OrderStatus.Processing) => true,
                (OrderStatus.Pending, OrderStatus.Cancelled) => true,
                (OrderStatus.Processing, OrderStatus.Shipped) => true,
                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                _ => false
            };
        }
    }
}
