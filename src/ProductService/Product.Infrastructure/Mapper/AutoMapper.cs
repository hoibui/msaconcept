using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Product.Infrastructure.Mapper;

public static class AutoMapper
{
    public static IServiceCollection AddAutoMappers(this IServiceCollection services)
    {
        var configuration = new MapperConfiguration(config => { config.AddProfile(new MappingProfile()); });

        var mapper = configuration.CreateMapper();

        return services.AddSingleton(mapper);
    }
}