using System.Diagnostics.Metrics;

namespace Api;

public class MetricsConfig
{
    public const string ServiceName = "minitwit-api";
    public const string ServiceVersion = "1.0.0";
    static readonly Meter Meter = new (ServiceName, ServiceVersion);
    public readonly Counter<int> MessagesCounter = Meter.CreateCounter<int>("messages_total");
    public readonly Counter<int> RegisterCounter = Meter.CreateCounter<int>("registers_total");
    public readonly Counter<int> FollowCounter = Meter.CreateCounter<int>("follows_total");
    public readonly Counter<int> UnfollowCounter = Meter.CreateCounter<int>("unfollows_total");
    public readonly Counter<int> CreateFaultyFollow = Meter.CreateCounter<int>("create_faulty_follow");
    public readonly Counter<int> CreateFaultyFollower = Meter.CreateCounter<int>("create_faulty_follower");
    public readonly Counter<int> CreateFaultyMessager = Meter.CreateCounter<int>("create_faulty_messager");
}