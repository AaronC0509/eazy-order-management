using CustomerPortal.Exceptions;
using CustomerPortal.Extensions;
using CustomerPortal.Models.DTO;
using CustomerPortal.Models.Enum;
using CustomerPortal.Models.Response;
using CustomerPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortal.Controllers
{
    [ApiController]
    [Route("api/order")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var customerId = Guid.Parse(User.GetUserId());
                var order = await _orderService.CreateOrderAsync(customerId, request);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(Guid id)
        {
            try
            {
                var customerId = Guid.Parse(User.GetUserId());
                var order = await _orderService.GetOrderByIdAsync(id, customerId);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetCustomerOrders()
        {
            try
            {
                var customerId = Guid.Parse(User.GetUserId());
                var orders = await _orderService.GetCustomerOrdersAsync(customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer orders");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<OrderResponseDto>> CancelOrder(Guid id)
        {
            try
            {
                var customerId = Guid.Parse(User.GetUserId());
                var order = await _orderService.CancelOrderAsync(id, customerId);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
