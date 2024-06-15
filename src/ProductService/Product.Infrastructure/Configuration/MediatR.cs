using System.Reflection;
using Common.ApiResponse;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Product.Infrastructure.Configuration;

public static class MediatR
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        return services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    public static void AddService<TRequest, TImplementation>(this IServiceCollection services)
        where TRequest : class, IRequest<ApiResult>
        where TImplementation : class, IRequestHandler<TRequest, ApiResult>
    {
        services.AddScoped<IRequestHandler<TRequest, ApiResult>, TImplementation>();
    }
}