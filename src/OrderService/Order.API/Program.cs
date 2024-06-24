using Common.Middlewares;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Order.API;
using Order.API.Db;
using Orders.API.Extensions;
using Plain.RabbitMQ;
using RabbitMQ.Client;

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
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
});


builder.Services.AddHostedService<CatalogResponseListener>();

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

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();