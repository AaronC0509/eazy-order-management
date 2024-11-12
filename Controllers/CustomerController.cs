using CustomerPortal.Exceptions;
using CustomerPortal.Extensions;
using CustomerPortal.Models.DTO;
using CustomerPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortal.Controllers
{
    [ApiController]
    [Route("api/customer")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            ICustomerService customerService,
            ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<CustomerDto>> GetProfile()
        {
            try
            {
                var userId = User.GetUserId();
                var customer = await _customerService.GetCustomerByIdAsync(Guid.Parse(userId));
                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer profile");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerDto>> Register([FromBody] RegisterCustomerRequest request)
        {
            try
            {
                if (request.Password != request.ConfirmPassword)
                {
                    return BadRequest("The password and confirmation password do not match.");
                }

                var customerDto = new CustomerDto
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address
                };

                var createdCustomer = await _customerService.CreateCustomerAsync(customerDto, request.Password);

                return CreatedAtAction(
                    nameof(GetProfile),
                    new { id = createdCustomer.Id },
                    createdCustomer);
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering new customer");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            try
            {
                if (!User.IsAdmin() && User.GetUserId() != id.ToString())
                {
                    return Forbid();
                }

                var customerDto = new CustomerDto
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address
                };

                var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customerDto);
                return Ok(updatedCustomer);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer with ID: {CustomerId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (User.GetUserId() != id.ToString())
                    return Forbid();

                var result = await _customerService.ChangePasswordAsync(
                    id,
                    request.CurrentPassword,
                    request.NewPassword);

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
                _logger.LogError(ex, "Error changing password for customer with ID: {CustomerId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
