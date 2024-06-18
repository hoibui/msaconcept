using DataHelper.EntityFramework.UnitOfwork.Implements;
using DataHelper.EntityFramework.UnitOfwork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Product.Data.DbContext;
using Product.Service.Commands;
using System.Collections.Generic;
using Product.Service.Queries;

namespace Product.Infrastructure.Configuration;

public static class DependencyServices
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Environment.GetEnvironmentVariable("REDIS_HOST");
        });


        #region Product
        
        services.AddService<CreateProductRequest, CreateProductHandler>();
        services.AddService<GetProductRequest, GetProductHandler>();
        services.AddService<GetProductTrendRequest, GetProductTrendHandler>();
        
        #endregion

        return services;
    }
    
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        var readConnectionString = Environment.GetEnvironmentVariable("READ_DATABASE_CONNECTION_STRING");
        var writeConnectionString = Environment.GetEnvironmentVariable("WRITE_DATABASE_CONNECTION_STRING");

        services.AddDbContext<ReadDbContext>(opt => opt.UseNpgsql(readConnectionString));
        services.AddScoped<IUnitOfWork<ReadDbContext>, UnitOfWork<ReadDbContext>>();

        services.AddDbContext<WriteDbContext>(opt => opt.UseNpgsql(writeConnectionString));
        services.AddScoped<IUnitOfWork<WriteDbContext>, UnitOfWork<WriteDbContext>>();

        return services;
    }
}