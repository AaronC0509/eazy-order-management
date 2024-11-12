using CustomerPortal.Exceptions;
using CustomerPortal.Models.Common;
using CustomerPortal.Models.DTO;
using CustomerPortal.Models.Enum;
using CustomerPortal.Models.Response;
using CustomerPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortal.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ICustomerService customerService,
            IOrderService orderService,
            ILogger<AdminController> logger)
        {
            _customerService = customerService;
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet("customers")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new PaginationFilter
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var customers = await _customerService.GetAllCustomersAsync(filter);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("customers/{id}/deactivate")]
        public async Task<IActionResult> DeactivateCustomer(Guid id)
        {
            try
            {
                await _customerService.DeactivateCustomerAsync(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating customer {CustomerId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("customers/{id}/activate")]
        public async Task<IActionResult> ActivateCustomer(Guid id)
        {
            try
            {
                await _customerService.ActivateCustomerAsync(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating customer {CustomerId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAllOrders([FromQuery] OrderStatus? status = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new PaginationFilter
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var orders = status.HasValue
                    ? await _orderService.GetOrdersByStatusAsync(status.Value, filter)
                    : await _orderService.GetAllOrdersAsync(filter);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("orders/{id}/status")]
        public async Task<ActionResult<OrderResponseDto>> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, request.Status);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status {OrderId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
