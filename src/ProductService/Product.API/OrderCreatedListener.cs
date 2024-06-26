using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using Product.Data.DbContext;

namespace Product.API;

public class OrderCreatedListener : IHostedService
    {
        private readonly ISubscriber _subscribe;
        private readonly IPublisher _publisher;
        private readonly IServiceScopeFactory _scopeFactory;
        public OrderCreatedListener(ISubscriber subscriber, IPublisher publisher, IServiceScopeFactory scopeFactory)
        {
            _subscribe = subscriber;
            _publisher = publisher;
            _scopeFactory = scopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscribe.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<OrderRequest>(message);

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
                try
                {
                    Product.Domain.Entities.Product catalogItem = _context.Products.Find(response.CatalogId);

                    if (catalogItem == null || catalogItem.Stock < response.Units)
                        throw new Exception();

                    catalogItem.Stock = catalogItem.Stock - response.Units;
                    _context.Entry(catalogItem).State = EntityState.Modified;
                    _context.SaveChanges();

                    _publisher.Publish(JsonConvert.SerializeObject(
                            new CatalogResponse { OrderId = response.OrderId, CatalogId = response.CatalogId, IsSuccess = true }
                        ), "catalog_response_routingkey", null);
                }
                catch (Exception)
                {
                    _publisher.Publish(JsonConvert.SerializeObject(
                    new CatalogResponse { OrderId = response.OrderId, CatalogId = response.CatalogId, IsSuccess = false }
                ), "catalog_response_routingkey", null);
                }
            }

            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }