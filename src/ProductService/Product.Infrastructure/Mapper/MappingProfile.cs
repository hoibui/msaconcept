using AutoMapper;
using Product.Service.Commands;

namespace Product.Infrastructure.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product
        CreateMap<CreateProductRequest, Domain.Entities.Product>();
    }
}
