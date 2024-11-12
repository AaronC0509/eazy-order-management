using AutoMapper;
using CustomerPortal.Data;
using CustomerPortal.Exceptions;
using CustomerPortal.Models.Common;
using CustomerPortal.Models.DTO;
using CustomerPortal.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Services.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Customer> _passwordHasher;
        private readonly IMapper _mapper;

        public CustomerService(
            ApplicationDbContext context,
            IPasswordHasher<Customer> passwordHasher,
            IMapper mapper)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == id);

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto, string password)
        {
            var customer = _mapper.Map<Customer>(customerDto);
            customer.CreatedAt = DateTime.UtcNow;
            customer.IsActive = true;
            customer.Role = "Customer";
            customer.PasswordHash = _passwordHasher.HashPassword(customer, password);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> UpdateCustomerAsync(Guid id, CustomerDto customerDto)
        {
            var customer = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                throw new NotFoundException($"Customer with ID {id} not found");

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.PhoneNumber = customerDto.PhoneNumber;
            customer.UpdatedAt = DateTime.UtcNow;

            if (customerDto.Address != null)
            {
                if (customer.Address == null)
                {
                    customer.Address = new Address
                    {
                        CustomerId = customer.Id
                    };
                }

                customer.Address.Street = customerDto.Address.Street;
                customer.Address.City = customerDto.Address.City;
                customer.Address.State = customerDto.Address.State;
                customer.Address.PostalCode = customerDto.Address.PostalCode;
                customer.Address.Country = customerDto.Address.Country;
            }

            try
            {
                await _context.SaveChangesAsync();
                return _mapper.Map<CustomerDto>(customer);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExists(id))
                    throw new NotFoundException($"Customer with ID {id} not found");
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return false;

            var hasOrders = await _context.Orders
                .AnyAsync(o => o.CustomerId == id);

            if (hasOrders)
            {
                customer.IsActive = false;
                customer.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid id, string currentPassword, string newPassword)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                throw new NotFoundException($"Customer with ID {id} not found");

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                customer,
                customer.PasswordHash,
                currentPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Current password is incorrect");

            customer.PasswordHash = _passwordHasher.HashPassword(customer, newPassword);
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResponse<CustomerDto>> GetAllCustomersAsync(PaginationFilter filter)
        {
            var query = _context.Customers
                .Include(c => c.Address)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking();

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<CustomerDto>>(items);

            return new PagedResponse<CustomerDto>(dtos, filter.PageNumber, filter.PageSize, totalItems);
        }

        public async Task<bool> DeactivateCustomerAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                throw new NotFoundException($"Customer with ID {id} not found");

            if (customer.Role == "Admin")
                throw new InvalidOperationException("Admin accounts cannot be deactivated");

            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateCustomerAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                throw new NotFoundException($"Customer with ID {id} not found");

            customer.IsActive = true;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> CustomerExists(Guid id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }
    }
}
