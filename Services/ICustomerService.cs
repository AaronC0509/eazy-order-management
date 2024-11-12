using CustomerPortal.Models.Common;
using CustomerPortal.Models.DTO;

namespace CustomerPortal.Services
{
    public interface ICustomerService
    {
        Task<CustomerDto> GetCustomerByIdAsync(Guid id);
        Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto, string password);
        Task<CustomerDto> UpdateCustomerAsync(Guid id, CustomerDto customerDto);
        Task<bool> DeleteCustomerAsync(Guid id);
        Task<bool> ChangePasswordAsync(Guid id, string currentPassword, string newPassword);
        Task<PagedResponse<CustomerDto>> GetAllCustomersAsync(PaginationFilter filter);
        Task<bool> DeactivateCustomerAsync(Guid id);
        Task<bool> ActivateCustomerAsync(Guid id);
    }
}
