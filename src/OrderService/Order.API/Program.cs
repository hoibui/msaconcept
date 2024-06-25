using Common.Middlewares;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Order.API;
using Order.API.Controllers.v1;
using Order.API.Db;
using Orders.API.Extensions;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAsymmetricAuthentication();
builder.Services.AddExceptionHandler<ExceptionMiddleware>();
builder.Services.AddProblemDetails();
// Configure database
var writeConnectionString = Environment.GetEnvironmentVariable("WRITE_DATABASE_CONNECTION_STRING");
builder.Services.AddDbContext<OrderingContext>(options => options.UseNpgsql(writeConnectionString));


//Configure rabbitmq
var rabbitMqHost = Environment.GetEnvironmentVariable("RABBIT_MQ");
builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider(rabbitMqHost));

builder.Services.AddSingleton<IPublisher>(p => new Publisher(p.GetService<IConnectionProvider>(),
    "order_exchange", // exchange name
    ExchangeType.Topic));

builder.Services.AddSingleton<ISubscriber>(s => new Subscriber(s.GetService<IConnectionProvider>(),
    "catalog_exchange", // Exchange name
    "catalog_response_queue", //queue name
    "catalog_response_routingkey", // routing key
    ExchangeType.Topic));


builder.Services.AddOpenTelemetry().WithTracing(b =>
{
    b.SetResourceBuilder(
            ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
        .AddSource(nameof(OrderItemsController))
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OrderServices"))
        .AddAspNetCoreInstrumentation(o =>
        {
            // to trace only api requests
            //o.Filter = (context) => !string.IsNullOrEmpty(context.Request.Path.Value) && context.Request.Path.Value.Contains("Api", StringComparison.InvariantCulture);

            // example: only collect telemetry about HTTP GET requests
            // return httpContext.Request.Method.Equals("GET");

            // enrich activity with http request and response
            o.EnrichWithHttpRequest = (activity, httpRequest) => { activity.SetTag("requestProtocol", httpRequest.Protocol); };
            o.EnrichWithHttpResponse = (activity, httpResponse) => { activity.SetTag("responseLength", httpResponse.ContentLength); };

            // automatically sets Activity Status to Error if an unhandled exception is thrown
            o.RecordException = true;
            o.EnrichWithException = (activity, exception) =>
            {
                activity.SetTag("exceptionType", exception.GetType().ToString());
                activity.SetTag("stackTrace", exception.StackTrace);
            };
        })
        .AddEntityFrameworkCoreInstrumentation(opt =>
        {
            opt.SetDbStatementForText = true;
            opt.SetDbStatementForStoredProcedure = true;
            opt.EnrichWithIDbCommand = (activity, command) =>
            {
                var stateDisplayName = $"{command.CommandType} main";
                activity.DisplayName = stateDisplayName;
                activity.SetTag("db.name", stateDisplayName);
            };
        })
        .AddConsoleExporter()
        .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
});




/*
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});
*/
builder.Services.AddHostedService<CatalogResponseListener>();
builder.Host.UseSerilog(SeriLogger.Configure);
var app = builder.Build();
app.UseExceptionHandler(builder =>
{

});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<LogContextMiddleware>();
app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();