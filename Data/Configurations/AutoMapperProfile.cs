using AutoMapper;
using CustomerPortal.Models.DTO;
using CustomerPortal.Models.Entities;
using CustomerPortal.Models.Response;

namespace CustomerPortal.Data.Configurations
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, CustomerDto>();

            CreateMap<Customer, CustomerDto>().ReverseMap();

            CreateMap<Address, AddressDto>().ReverseMap();

            CreateMap<Order, OrderDto>();

            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<Order, OrderResponseDto>();
        }
    }
}
