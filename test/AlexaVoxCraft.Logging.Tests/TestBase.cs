using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit3;
using NSubstitute;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.TestCorrelator;
using System.Reflection;

namespace AlexaVoxCraft.Logging.Tests;

/// <summary>
/// Base class for all Logging tests providing common setup and utilities.
/// </summary>
public abstract class TestBase
{
    protected IFixture Fixture { get; }

    protected TestBase()
    {
        Fixture = new Fixture();
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        // Add custom specimen builder for LogEvents
        Fixture.Customizations.Add(new LogEventSpecimenBuilder());
    }

    /// <summary>
    /// Creates a substitute for the specified type.
    /// </summary>
    protected T CreateSubstitute<T>() where T : class => Substitute.For<T>();

    /// <summary>
    /// Creates a test logger configured to capture events for testing.
    /// </summary>
    protected ILogger CreateTestLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.TestCorrelator()
            .CreateLogger();
    }

    /// <summary>
    /// Gets all log events captured during the test.
    /// </summary>
    protected IEnumerable<LogEvent> GetLogEvents() => TestCorrelator.GetLogEventsFromCurrentContext();

    /// <summary>
    /// Gets log events of a specific level.
    /// </summary>
    protected IEnumerable<LogEvent> GetLogEvents(LogEventLevel level) =>
        GetLogEvents().Where(e => e.Level == level);
}

/// <summary>
/// AutoData attribute that uses a customized fixture for consistent test data generation.
/// </summary>
public class AutoDataAttribute : AutoFixture.Xunit3.AutoDataAttribute
{
    public AutoDataAttribute() : base(() => CreateFixture()) { }

    private static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customizations.Add(new LogEventSpecimenBuilder());
        return fixture;
    }
}

/// <summary>
/// Custom specimen builder that creates LogEvent instances based on parameter naming conventions.
/// </summary>
public class LogEventSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo parameter || parameter.ParameterType != typeof(LogEvent))
            return new NoSpecimen();

        var parameterName = parameter.Name?.ToLowerInvariant() ?? "";
        
        return parameterName switch
        {
            var name when name.Contains("basic") => CreateBasicLogEvent(),
            var name when name.Contains("information") => CreateInformationLogEvent(),
            var name when name.Contains("warning") => CreateWarningLogEvent(),
            var name when name.Contains("error") => CreateErrorLogEvent(),
            var name when name.Contains("exception") => CreateLogEventWithException(),
            var name when name.Contains("properties") => CreateLogEventWithProperties(),
            var name when name.Contains("template") => CreateLogEventWithTemplate(),
            var name when name.Contains("rendering") => CreateLogEventWithRenderings(),
            var name when name.Contains("traceid") || name.Contains("trace") => CreateLogEventWithTraceId(),
            var name when name.Contains("spanid") || name.Contains("span") => CreateLogEventWithSpanId(),
            var name when name.Contains("timestamp") => CreateLogEventWithTimestamp(),
            var name when name.Contains("complex") => CreateLogEventWithComplexProperties(),
            var name when name.Contains("multiline") => CreateLogEventWithMultilineProperties(),
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
            Array.Empty<LogEventProperty>());
    }

    private static LogEvent CreateInformationLogEvent()
    {
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("Information level message"),
            Array.Empty<LogEventProperty>());
    }

    private static LogEvent CreateWarningLogEvent()
    {
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Warning,
            null,
            new MessageTemplateParser().Parse("Warning level message"),
            Array.Empty<LogEventProperty>());
    }

    private static LogEvent CreateErrorLogEvent()
    {
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Error,
            null,
            new MessageTemplateParser().Parse("Error level message"),
            Array.Empty<LogEventProperty>());
    }

    private static LogEvent CreateLogEventWithException()
    {
        var exception = new InvalidOperationException("Test exception");
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Error,
            exception,
            new MessageTemplateParser().Parse("Error occurred"),
            Array.Empty<LogEventProperty>());
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
            Array.Empty<LogEventProperty>());
    }

    private static LogEvent CreateLogEventWithComplexProperties()
    {
        var properties = new[]
        {
            new LogEventProperty("Data", new StructureValue(new[]
            {
                new LogEventProperty("Name", new ScalarValue("Test")),
                new LogEventProperty("Value", new ScalarValue(42)),
                new LogEventProperty("Active", new ScalarValue(true))
            }))
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