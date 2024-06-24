using Microsoft.AspNetCore.Mvc;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Order.API.Db;
using Order.API.Models;
using Plain.RabbitMQ;

namespace Order.API.Controllers.v1;

[Route("api/v1/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
public class OrderItemsController : ControllerBase
{
    private readonly OrderingContext _context;
    private readonly IPublisher _publisher;

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

        
    // POST: api/OrderItems
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task PostOrderItem(OrderItem orderItem)
    {
        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();

        // New inserted identity value
        Guid id = orderItem.Id;


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