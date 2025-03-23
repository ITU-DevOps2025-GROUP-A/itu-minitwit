using System.Diagnostics.Metrics;

namespace Api;

public class MetricsConfig
{
    public const string ServiceName = "minitwit-api";
    public const string ServiceVersion = "1.0.0";
    static readonly Meter Meter = new (ServiceName, ServiceVersion);
}