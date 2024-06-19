using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Consul;

namespace Product.Infrastructure.Configuration;

public static class ConsulServiceExtension
    {
        

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("AppExtensions");
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            if (app.Properties["server.Features"] is not FeatureCollection features)
            {
                return app;
            }



            var servicePort = int.Parse("5064");
           // var serviceIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            var serviceName = "ProductService";
            var serviceId = serviceName + "-" + Guid.NewGuid();

            var registration = new AgentServiceRegistration()
            {
                ID = serviceName,
                Name = serviceName,
                //Address = serviceIp.ToString(),
                Address =  "localhost",
                Port = servicePort,

                Check = new AgentCheckRegistration()
                {
                    HTTP = $"http://localhost:{servicePort}/hc",
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            });

            return app;
        }
    }