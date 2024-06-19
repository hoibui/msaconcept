using System.Net.Mime;
using Common.Middlewares;
using Plain.RabbitMQ;
using Product.API;
using Product.Infrastructure.Configuration;
using Product.Infrastructure.Mapper;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var rabbitMqHost = Environment.GetEnvironmentVariable("RABBIT_MQ");
builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider(rabbitMqHost));
builder.Services.AddSingleton<IPublisher>(p => new Publisher(p.GetService<IConnectionProvider>(),
    "catalog_exchange",
    ExchangeType.Topic));

builder.Services.AddSingleton<ISubscriber>(s => new Subscriber(s.GetService<IConnectionProvider>(),
    "order_exchange",
    "order_response_queue",
    "order_created_routingkey",
    ExchangeType.Topic
));

builder.Services.AddHostedService<OrderCreatedListener>();

builder.Services.AddExceptionHandler<ExceptionMiddleware>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddUnitOfWork();
builder.Services.AddProblemDetails();
builder.Services.AddMediator();
builder.Services.AddAutoMappers();
builder.Services.AddServices();

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