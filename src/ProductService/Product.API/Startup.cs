using System;
using Common.Middlewares;
using Product.API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Product.Infrastructure.Configuration;
using Product.Infrastructure.Mapper;

namespace Product.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling =
                        ReferenceLoopHandling.Ignore;
                });

            services.AddAsymmetricAuthentication();
            
            var rabbitMqHost = Environment.GetEnvironmentVariable("RABBIT_MQ");
            services.AddSingleton<IConnectionProvider>(new ConnectionProvider(rabbitMqHost));
            services.AddSingleton<IPublisher>(p => new Publisher(p.GetService<IConnectionProvider>(),
                "catalog_exchange",
                ExchangeType.Topic));

            services.AddSingleton<ISubscriber>(s => new Subscriber(s.GetService<IConnectionProvider>(),
                "order_exchange",
                "order_response_queue",
                "order_created_routingkey",
                ExchangeType.Topic
            ));
            
            services.AddOpenTelemetry().WithTracing(b =>
            {
                b.SetResourceBuilder(
                        ResourceBuilder.CreateDefault().AddService("Product"))
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
            });
            
            services.AddHostedService<OrderCreatedListener>();
            services.AddExceptionHandler<ExceptionMiddleware>();
            services.AddEndpointsApiExplorer();
            services.AddUnitOfWork();
            services.AddProblemDetails();
            services.AddMediator();
            services.AddAutoMappers();
            services.AddServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}