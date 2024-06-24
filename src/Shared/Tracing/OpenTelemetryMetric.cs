using System.Diagnostics.Metrics;

namespace Tracing;

public static class OpenTelemetryMetric
{
    // Identity metrics
    private static readonly Meter IdentityMeter = new("Product.Api");
    public static readonly Counter<int> ProducteCreateEventCounter = IdentityMeter.CreateCounter<int>("product.created.event.count");

}