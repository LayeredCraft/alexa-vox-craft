using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AlexaVoxCraft.MediatR.Observability;

public static class AlexaVoxCraftTelemetry
{
    public const string ActivitySourceName = "AlexaVoxCraft";
    public const string MeterName = "AlexaVoxCraft";
    
    public static readonly ActivitySource Source = new(ActivitySourceName, "1.0.0");
    public static readonly Meter Meter = new(MeterName, "1.0.0");

    public static readonly Counter<long> Requests = 
        Meter.CreateCounter<long>(AlexaMetricNames.Requests);
    public static readonly Counter<long> Errors = 
        Meter.CreateCounter<long>(AlexaMetricNames.Errors);
    public static readonly Counter<long> ColdStarts = 
        Meter.CreateCounter<long>(AlexaMetricNames.ColdStarts);
    public static readonly Counter<long> SlotResolutions = 
        Meter.CreateCounter<long>(AlexaMetricNames.SlotResolutions);
    public static readonly Counter<long> SkillVerificationFailures = 
        Meter.CreateCounter<long>(AlexaMetricNames.SkillVerificationFailures);

    public static readonly Histogram<double> Latency = 
        Meter.CreateHistogram<double>(AlexaMetricNames.Latency, unit: "ms");
    public static readonly Histogram<double> HandlerDuration = 
        Meter.CreateHistogram<double>(AlexaMetricNames.HandlerDuration, "ms");
    public static readonly Histogram<double> SerializationDuration = 
        Meter.CreateHistogram<double>(AlexaMetricNames.SerializationDuration, "ms");
    public static readonly Histogram<long> ResponseSize = 
        Meter.CreateHistogram<long>(AlexaMetricNames.ResponseSize);
    public static readonly Histogram<long> SpeechCharacters = 
        Meter.CreateHistogram<long>(AlexaMetricNames.SpeechCharacters);
    public static readonly Histogram<double> AplRenderDuration = 
        Meter.CreateHistogram<double>(AlexaMetricNames.AplRenderDuration, "ms");

    private static int _coldStart = 1;
    public static bool IsColdStart() => Interlocked.Exchange(ref _coldStart, 0) == 1;
}