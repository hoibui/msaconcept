using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Serilog;

namespace Orders.API.Extensions;


public static class SeriLogger
{
    public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
        (context, configuration) =>
        {

            configuration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new [] { new Uri("http://localhost:9200" )}, opts =>
                {
                   // opts.DataStream = new DataStreamName("logs", "console-example", "demo");
                   // opts.BootstrapMethod = BootstrapMethod.Failure;
                    opts.ConfigureChannel = channelOpts =>
                    {
                        channelOpts.BufferOptions = new BufferOptions
                        {
                            ExportMaxConcurrency = 10
                        };
                    };
                })
                .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                .Enrich.WithProperty("ContentRootPath", context.HostingEnvironment.ContentRootPath)
                .Enrich.WithEnvironmentName()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(context.Configuration);
        };
}