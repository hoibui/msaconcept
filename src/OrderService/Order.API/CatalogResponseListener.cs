using Newtonsoft.Json;
using Plain.RabbitMQ;
using Common.Models;
using Order.API.Db;

namespace Order.API;

public class CatalogResponseListener : IHostedService
{
    private ISubscriber _subscriber;
    private readonly IServiceScopeFactory _scopeFactory;
    public CatalogResponseListener(ISubscriber subscripber, IServiceScopeFactory scopeFactory)
    {
        this._subscriber = subscripber;
        this._scopeFactory = scopeFactory;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _subscriber.Subscribe(Subscribe);
        return Task.CompletedTask;
    }

    private bool Subscribe(string message, IDictionary<string, object> header)
    {
        var response = JsonConvert.DeserializeObject<CatalogResponse>(message);

        if(!response.IsSuccess)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _orderingContext = scope.ServiceProvider.GetRequiredService<OrderingContext>();
                    
                // If transaction is not successful, Remove ordering item
                var orderItem = _orderingContext.OrderItems.Where(o => o.ProductId == response.CatalogId && o.OrderId == response.OrderId).FirstOrDefault();
                _orderingContext.OrderItems.Remove(orderItem);
                _orderingContext.SaveChanges();
            }
        }
        return true;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}