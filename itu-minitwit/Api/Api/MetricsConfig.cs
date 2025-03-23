using System.Diagnostics.Metrics;

namespace Api;

public class MetricsConfig
{
    public const string ServiceName = "minitwit-api";
    public const string ServiceVersion = "1.0.0";
    static readonly Meter Meter = new (ServiceName, ServiceVersion);
    public readonly Counter<int> MessagesCounter = Meter.CreateCounter<int>("messages_total");
    public readonly Counter<int> RegisterCounter = Meter.CreateCounter<int>("registers_total");
}