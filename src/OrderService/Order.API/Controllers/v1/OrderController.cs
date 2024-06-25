using Microsoft.AspNetCore.Mvc;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Order.API.Db;
using Order.API.Models;
using Plain.RabbitMQ;
using RabbitMQ.Client;

namespace Order.API.Controllers.v1;

[Route("api/v1/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
public class OrderItemsController : ControllerBase
{
    private readonly OrderingContext _context;
    private readonly IPublisher _publisher;
    private static readonly ActivitySource Activity = new(nameof(OrderItemsController));
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;


    public OrderItemsController(OrderingContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    // GET: api/OrderItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
    {
        return await _context.OrderItems.ToListAsync();
    }

    // GET: api/OrderItems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
    {
     
            var orderItem = await _context.OrderItems.FindAsync(id);

           
            
            
            if (orderItem == null)
            {
                return NotFound();
            }


            return orderItem;
        
    }
    
    private void AddActivityToHeader(Activity activity, IBasicProperties props)
    {
        Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), props, InjectContextIntoHeader);
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination_kind", "queue");
        activity?.SetTag("messaging.rabbitmq.queue", "sample");
    }

    private void InjectContextIntoHeader(IBasicProperties props, string key, string value)
    {
        try
        {
            props.Headers ??= new Dictionary<string, object>();
            props.Headers[key] = value;
        }
        catch (Exception ex)
        {
           Console.WriteLine($"Failed to inject trace context. - {ex}");
        }
    }

        
    // POST: api/OrderItems
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task PostOrderItem(OrderItem orderItem)
    {
        using (var activity = Activity.StartActivity("Create new Order", ActivityKind.Server))
        {
            activity?.AddEvent(new ("Create Order"));
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            activity?.AddEvent(new ("Create Order successfully"));
        }

        using (var activity = Activity.StartActivity("RabbitMq Publish", ActivityKind.Producer))
        {
            // New inserted identity value
            Guid id = orderItem.Id;

            var props = typeof(IPublisher).GetProperties().FirstOrDefault();

           // AddActivityToHeader(activity, props);

            _publisher.Publish(JsonConvert.SerializeObject(new OrderRequest
                {
                    OrderId = orderItem.OrderId,
                    CatalogId = orderItem.ProductId,
                    Units = orderItem.Units,
                    Name = orderItem.ProductName
                }),
                "order_created_routingkey", // Routing key
                null);
        }
    }
}