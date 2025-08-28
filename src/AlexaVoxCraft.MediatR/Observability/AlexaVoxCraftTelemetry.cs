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
        Meter.CreateHistogram<long>(AlexaMetricNames.ResponseSize, unit: "By");
    public static readonly Histogram<long> SpeechCharacters = 
        Meter.CreateHistogram<long>(AlexaMetricNames.SpeechCharacters, unit: "{character}");
    public static readonly Histogram<double> AplRenderDuration = 
        Meter.CreateHistogram<double>(AlexaMetricNames.AplRenderDuration, "ms");

    private static int _coldStart = 1;
    public static bool IsColdStart() => Interlocked.Exchange(ref _coldStart, 0) == 1;

    public static TimerScope TimeLatency() => new TimerScope(Latency);
    public static TimerScope TimeHandler() => new TimerScope(HandlerDuration);
    public static TimerScope TimeSerialization() => new TimerScope(SerializationDuration);
    public static TimerScope TimeAplRender() => new TimerScope(AplRenderDuration);

    public readonly struct TimerScope : IDisposable
    {
        private readonly Histogram<double> _h;
        private readonly long _start;
        private readonly KeyValuePair<string, object?>[]? _tags;

        public TimerScope(Histogram<double> h)
        {
            _h = h;
            _start = Stopwatch.GetTimestamp();
            _tags = null;
        }

        public TimerScope(Histogram<double> h, params KeyValuePair<string, object?>[] tags)
        {
            _h = h;
            _start = Stopwatch.GetTimestamp();
            _tags = tags;
        }

        public void Dispose()
        {
            var ms = (Stopwatch.GetTimestamp() - _start) * 1000.0 / Stopwatch.Frequency;
            if (_tags != null && _tags.Length > 0)
            {
                _h.Record(ms, _tags);
            }
            else
            {
                _h.Record(ms);
            }
        }
    }
}