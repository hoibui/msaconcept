using Common.Middlewares;
using Microsoft.EntityFrameworkCore;
using Order.API;
using Order.API.Db;
using Plain.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
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

builder.Services.AddHostedService<CatalogResponseListener>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();