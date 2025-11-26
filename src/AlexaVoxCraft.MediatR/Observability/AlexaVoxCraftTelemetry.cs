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

    public static readonly Histogram<double> Latency = 
        Meter.CreateHistogram<double>(AlexaMetricNames.Latency, unit: "ms");
    public static readonly Histogram<double> HandlerDuration = 
        Meter.CreateHistogram<double>(AlexaMetricNames.HandlerDuration, "ms");
    public static readonly Histogram<double> HandlerResolutionDuration = 
        Meter.CreateHistogram<double>(AlexaMetricNames.HandlerResolutionDuration, "ms");
    public static readonly Counter<long> HandlerExecutions = 
        Meter.CreateCounter<long>(AlexaMetricNames.HandlerExecutions);
    public static readonly Counter<long> HandlerResolutionAttempts = 
        Meter.CreateCounter<long>(AlexaMetricNames.HandlerResolutionAttempts);
    public static readonly Histogram<double> SerializationDuration = 
        Meter.CreateHistogram<double>(AlexaMetricNames.SerializationDuration, "ms");

    public static readonly Histogram<long> SpeechCharacters = 
        Meter.CreateHistogram<long>(AlexaMetricNames.SpeechCharacters, unit: "{character}");
    public static readonly Histogram<double> AplRenderDuration = 
        Meter.CreateHistogram<double>(AlexaMetricNames.AplRenderDuration, "ms");
    public static readonly Histogram<double> LambdaDuration = 
        Meter.CreateHistogram<double>(AlexaMetricNames.LambdaDuration, "ms");
    public static readonly Histogram<long> LambdaMemoryUsed = 
        Meter.CreateHistogram<long>(AlexaMetricNames.LambdaMemoryUsed, "MB");
    public static readonly Histogram<long> PayloadSize = 
        Meter.CreateHistogram<long>(AlexaMetricNames.PayloadSize, unit: "By");

    private static int _coldStart = 1;
    public static bool IsColdStart() => Interlocked.Exchange(ref _coldStart, 0) == 1;

    public static TimerScope TimeHandlerResolution() => new(HandlerResolutionDuration);
    public static TimerScope TimeHandlerExecution(string handlerType, bool isDefault = false) => 
        new(HandlerDuration, 
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerType, handlerType),
            new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerIsDefault, isDefault));
    public static TimerScope TimeRequestSerialization() => new(SerializationDuration, 
        new KeyValuePair<string, object?>(AlexaSemanticAttributes.SerializationDirection, AlexaSemanticValues.SerializationDirectionRequest));
    public static TimerScope TimeResponseSerialization() => new(SerializationDuration, 
        new KeyValuePair<string, object?>(AlexaSemanticAttributes.SerializationDirection, AlexaSemanticValues.SerializationDirectionResponse));

    public static TimerScope TimeLambda() => new(LambdaDuration);

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
    internal static void ResetForTesting()
    {
        Volatile.Write(ref _coldStart, 1);
    }
}