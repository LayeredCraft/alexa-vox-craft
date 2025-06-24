using System.Text.Json;
using AlexaVoxCraft.Logging.Serialization;
using AwesomeAssertions;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Parsing;

namespace AlexaVoxCraft.Logging.Tests;

public class AlexaCompactJsonFormatterTests : TestBase
{
    private readonly AlexaCompactJsonFormatter _formatter;
    
    public AlexaCompactJsonFormatterTests()
    {
        _formatter = new AlexaCompactJsonFormatter();
    }

    [Fact]
    public void Constructor_WithDefaultParameters_CreatesFormatter()
    {
        var formatter = new AlexaCompactJsonFormatter();
        
        formatter.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithCustomValueFormatter_CreatesFormatter()
    {
        var valueFormatter = new JsonValueFormatter();
        var formatter = new AlexaCompactJsonFormatter(valueFormatter);
        
        formatter.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullValueFormatter_CreatesFormatter()
    {
        var formatter = new AlexaCompactJsonFormatter(null);
        
        formatter.Should().NotBeNull();
    }

    [Theory]
    [AutoData]
    public void Format_WithValidLogEvent_WritesJsonToOutput(LogEvent basicLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(basicLogEvent, output);

        var result = output.ToString();
        result.Should().NotBeNullOrEmpty();
        result.Should().EndWith("\n");
    }

    [Theory]
    [AutoData]
    public void Format_WithBasicEvent_ProducesValidJson(LogEvent basicLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(basicLogEvent, output);

        var result = output.ToString().TrimEnd('\n');
        var isValidJson = IsValidJson(result);
        isValidJson.Should().BeTrue($"Output should be valid JSON: {result}");
    }

    [Theory]
    [AutoData]
    public void Format_WithInformationLevel_DoesNotIncludeLevelField(LogEvent informationLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(informationLogEvent, output);

        var result = output.ToString();
        result.Should().NotContain("\"_l\":");
    }

    [Theory]
    [AutoData]
    public void Format_WithNonInformationLevel_IncludesLevelField(LogEvent warningLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(warningLogEvent, output);

        var result = output.ToString();
        result.Should().Contain("\"_l\":\"Warning\"");
    }

    [Theory]
    [AutoData]
    public void Format_WithMessageTemplate_IncludesMessageTemplateField(LogEvent templateLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(templateLogEvent, output);

        var result = output.ToString();
        result.Should().Contain("\"_mt\":");
    }

    [Theory]
    [AutoData]
    public void Format_WithTimestamp_IncludesTimestampField(LogEvent timestampLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(timestampLogEvent, output);

        var result = output.ToString();
        result.Should().Contain("\"_t\":\"2023-10-15T14:30:00.0000000Z\"");
    }

    [Theory]
    [AutoData]
    public void Format_WithException_IncludesExceptionField(LogEvent exceptionLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(exceptionLogEvent, output);

        var result = output.ToString();
        result.Should().Contain("\"_x\":");
        result.Should().Contain("System.InvalidOperationException");
        result.Should().Contain("Test exception");
    }

    [Theory]
    [AutoData]
    public void Format_WithProperties_IncludesPropertiesCorrectly(LogEvent propertiesLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(propertiesLogEvent, output);

        var result = output.ToString();
        result.Should().Contain("\"UserId\":\"12345\"");
        result.Should().Contain("\"RequestId\":\"abc-def-ghi\"");
    }

    [Fact]
    public void Format_WithPropertyStartingWithAt_EscapesToUnderscore()
    {
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            ["@timestamp"] = new ScalarValue("2023-10-15"),
            ["@version"] = new ScalarValue("1.0")
        };
        var logEventProperties = properties.Select(kvp => new LogEventProperty(kvp.Key, kvp.Value)).ToArray();
        var logEvent = new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplateParser().Parse("Test message"),
            logEventProperties);
        var output = new StringWriter();

        _formatter.Format(logEvent, output);

        var result = output.ToString();
        result.Should().Contain("\"_timestamp\":\"2023-10-15\"");
        result.Should().Contain("\"_version\":\"1.0\"");
        result.Should().NotContain("\"@timestamp\":");
        result.Should().NotContain("\"@version\":");
    }

    [Theory]
    [AutoData]
    public void Format_WithComplexObjectProperty_SerializesCorrectly(LogEvent complexLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(complexLogEvent, output);

        var result = output.ToString();
        result.Should().Contain("\"Data\":");
        result.Should().Contain("\"Name\":\"Test\"");
        result.Should().Contain("\"Value\":42");
        result.Should().Contain("\"Active\":true");
    }

    [Theory]
    [AutoData]
    public void Format_WithRenderings_IncludesRenderingsField(LogEvent renderingLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(renderingLogEvent, output);

        var result = output.ToString();
        // Should include renderings array when format specifiers are present
        result.Should().Contain("\"_r\":");
    }

    [Theory]
    [AutoData]
    public void Format_WithTraceId_IncludesTraceIdField(LogEvent traceIdLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(traceIdLogEvent, output);

        var result = output.ToString();
        result.Should().Contain("\"_tr\":");
    }

    [Theory]
    [AutoData]
    public void Format_WithSpanId_IncludesSpanIdField(LogEvent spanIdLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(spanIdLogEvent, output);

        var result = output.ToString();
        result.Should().Contain("\"_sp\":");
    }

    [Theory]
    [AutoData]
    public void Format_WithNullOutput_ThrowsArgumentNullException(LogEvent basicLogEvent)
    {
        var exception = Record.Exception(() => _formatter.Format(basicLogEvent, null!));

        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public void Format_WithNullLogEvent_ThrowsArgumentNullException()
    {
        var output = new StringWriter();

        var exception = Record.Exception(() => _formatter.Format(null!, output));

        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [AutoData]
    public void FormatEvent_StaticMethod_ProducesValidJson(LogEvent basicLogEvent, JsonValueFormatter valueFormatter)
    {
        var output = new StringWriter();

        AlexaCompactJsonFormatter.FormatEvent(basicLogEvent, output, valueFormatter);

        var result = output.ToString();
        var isValidJson = IsValidJson(result);
        isValidJson.Should().BeTrue($"Output should be valid JSON: {result}");
    }

    [Theory]
    [AutoData]
    public void Format_OutputIsSingleLine_NoInternalNewlines(LogEvent multilineLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(multilineLogEvent, output);

        var result = output.ToString();
        var lines = result.Split('\n');
        // Should only have one content line plus final newline (empty string after split)
        lines.Length.Should().Be(2);
        lines[0].Should().NotBeNullOrEmpty(); // The JSON content
        lines[1].Should().Be(""); // Empty string after final newline
    }

    [Theory]
    [AutoData]
    public void Format_WithRandomProperties_ProducesValidJson(LogEvent propertiesLogEvent)
    {
        var output = new StringWriter();

        _formatter.Format(propertiesLogEvent, output);

        var result = output.ToString().TrimEnd('\n');
        var isValidJson = IsValidJson(result);
        isValidJson.Should().BeTrue($"Output should be valid JSON: {result}");
    }

    // Helper methods

    private static bool IsValidJson(string json)
    {
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}