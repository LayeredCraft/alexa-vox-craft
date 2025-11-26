using System.Reflection;
using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;
using Serilog.Events;
using Serilog.Parsing;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

/// <summary>
/// Custom specimen builder that creates LogEvent instances based on parameter naming conventions.
/// </summary>
public class LogEventSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public LogEventSpecimenBuilder() : this(new LogEventSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        var parameterName = request switch
        {
            ParameterInfo parameter => parameter.Name?.ToLowerInvariant() ?? "",
            Type => "",
            _ => throw new ArgumentException("Invalid request type", nameof(request))
        };

        return parameterName switch
        {
            _ when parameterName.Contains("basic") => CreateBasicLogEvent(),
            _ when parameterName.Contains("information") => CreateInformationLogEvent(),
            _ when parameterName.Contains("warning") => CreateWarningLogEvent(),
            _ when parameterName.Contains("error") => CreateErrorLogEvent(),
            _ when parameterName.Contains("exception") => CreateLogEventWithException(),
            _ when parameterName.Contains("properties") => CreateLogEventWithProperties(),
            _ when parameterName.Contains("template") => CreateLogEventWithTemplate(),
            _ when parameterName.Contains("rendering") => CreateLogEventWithRenderings(),
            _ when parameterName.Contains("traceid") || parameterName.Contains("trace") => CreateLogEventWithTraceId(),
            _ when parameterName.Contains("spanid") || parameterName.Contains("span") => CreateLogEventWithSpanId(),
            _ when parameterName.Contains("timestamp") => CreateLogEventWithTimestamp(),
            _ when parameterName.Contains("complex") => CreateLogEventWithComplexProperties(),
            _ when parameterName.Contains("multiline") => CreateLogEventWithMultilineProperties(),
            _ => CreateBasicLogEvent()
        };
    }

    private static LogEvent CreateBasicLogEvent(string message = "Test message")
    {
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse(message),
            []);
    }

    private static LogEvent CreateInformationLogEvent()
    {
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("Information level message"),
            []);
    }

    private static LogEvent CreateWarningLogEvent()
    {
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Warning,
            null,
            new MessageTemplateParser().Parse("Warning level message"),
            []);
    }

    private static LogEvent CreateErrorLogEvent()
    {
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Error,
            null,
            new MessageTemplateParser().Parse("Error level message"),
            []);
    }

    private static LogEvent CreateLogEventWithException()
    {
        var exception = new InvalidOperationException("Test exception");
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Error,
            exception,
            new MessageTemplateParser().Parse("Error occurred"),
            []);
    }

    private static LogEvent CreateLogEventWithProperties()
    {
        var properties = new[]
        {
            new LogEventProperty("UserId", new ScalarValue("12345")),
            new LogEventProperty("RequestId", new ScalarValue("abc-def-ghi"))
        };
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("User action"),
            properties);
    }

    private static LogEvent CreateLogEventWithTemplate()
    {
        var properties = new[]
        {
            new LogEventProperty("UserId", new ScalarValue(123))
        };
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("User {UserId:000} performed action"),
            properties);
    }

    private static LogEvent CreateLogEventWithRenderings()
    {
        var properties = new[]
        {
            new LogEventProperty("UserId", new ScalarValue(123))
        };
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("User {UserId:000} performed action"),
            properties);
    }

    private static LogEvent CreateLogEventWithTraceId()
    {
        var traceId = System.Diagnostics.ActivityTraceId.CreateRandom();
        var properties = new[]
        {
            new LogEventProperty("TraceId", new ScalarValue(traceId.ToString()))
        };
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("Traced operation"),
            properties,
            traceId,
            default);
    }

    private static LogEvent CreateLogEventWithSpanId()
    {
        var spanId = System.Diagnostics.ActivitySpanId.CreateRandom();
        var properties = new[]
        {
            new LogEventProperty("SpanId", new ScalarValue(spanId.ToString()))
        };
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("Spanned operation"),
            properties,
            default,
            spanId);
    }

    private static LogEvent CreateLogEventWithTimestamp()
    {
        var timestamp = new DateTimeOffset(2023, 10, 15, 14, 30, 0, TimeSpan.Zero);
        return new LogEvent(
            timestamp,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("Test"),
            []);
    }

    private static LogEvent CreateLogEventWithComplexProperties()
    {
        var properties = new[]
        {
            new LogEventProperty("Data", new StructureValue([
                new LogEventProperty("Name", new ScalarValue("Test")),
                new LogEventProperty("Value", new ScalarValue(42)),
                new LogEventProperty("Active", new ScalarValue(true))
            ]))
        };
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("Complex data"),
            properties);
    }

    private static LogEvent CreateLogEventWithMultilineProperties()
    {
        var properties = new[]
        {
            new LogEventProperty("MultiLineText", new ScalarValue("Line 1\nLine 2\nLine 3")),
            new LogEventProperty("Description", new ScalarValue("Some\r\nWindows\r\nLinebreaks"))
        };
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("Multi-line test"),
            properties);
    }
}