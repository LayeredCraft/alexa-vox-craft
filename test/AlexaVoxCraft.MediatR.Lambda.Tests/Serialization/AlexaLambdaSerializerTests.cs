using System.Text;
using System.Text.Json;
using AlexaVoxCraft.MediatR.Lambda.Serialization;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Serialization;
using Microsoft.Extensions.Logging;
using LayeredCraft.StructuredLogging.Testing;

namespace AlexaVoxCraft.MediatR.Lambda.Tests.Serialization;

[Collection("DiagnosticsConfig Tests")]
public class AlexaLambdaSerializerTests : TestBase
{
    private readonly TestLogger<AlexaLambdaSerializer> _testLogger;
    private readonly AlexaLambdaSerializer _serializer;

    public AlexaLambdaSerializerTests()
    {
        _testLogger = new TestLogger<AlexaLambdaSerializer>();
        _serializer = new AlexaLambdaSerializer(_testLogger, null);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        var exception = Record.Exception(() => new AlexaLambdaSerializer(null!, null));
        
        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithCustomOptions_UsesProvidedOptions()
    {
        var customOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        
        var serializer = new AlexaLambdaSerializer(_testLogger, customOptions);
        
        serializer.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullOptions_UsesDefaultOptions()
    {
        var serializer = new AlexaLambdaSerializer(_testLogger, null);
        
        serializer.Should().NotBeNull();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void Deserialize_WithValidJson_ReturnsDeserializedObject(SkillRequest skillRequest)
    {
        var json = JsonSerializer.Serialize(skillRequest, AlexaJsonOptions.DefaultOptions);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        
        var result = _serializer.Deserialize<SkillRequest>(stream);
        
        result.Should().NotBeNull();
        result!.Request.Should().NotBeNull();
        result.Context.Should().NotBeNull();
    }

    [Fact]
    public void Deserialize_WithInvalidJson_ThrowsJsonException()
    {
        var invalidJson = "{ invalid json }";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(invalidJson));
        
        var exception = Record.Exception(() => _serializer.Deserialize<SkillRequest>(stream));
        
        exception.Should().NotBeNull();
        exception.Should().BeOfType<JsonException>();
    }

    [Fact]
    public void Deserialize_WithNullStream_ThrowsException()
    {
        var exception = Record.Exception(() => _serializer.Deserialize<SkillRequest>(null!));
        
        exception.Should().NotBeNull();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void Serialize_WithValidObject_WritesToStream(SkillResponse skillResponse)
    {
        using var stream = new MemoryStream();
        
        _serializer.Serialize(skillResponse, stream);
        
        stream.Length.Should().BeGreaterThan(0);
        stream.Position.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Serialize_WithNullStream_ThrowsException()
    {
        var response = new SkillResponse();
        
        var exception = Record.Exception(() => _serializer.Serialize(response, null!));
        
        exception.Should().NotBeNull();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void Serialize_WithObject_ProducesValidJson(SkillResponse skillResponse)
    {
        using var stream = new MemoryStream();
        
        _serializer.Serialize(skillResponse, stream);
        
        stream.Position = 0;
        var json = new StreamReader(stream).ReadToEnd();
        json.Should().NotBeNullOrEmpty();
        
        // Verify it's valid JSON by trying to parse it back
        var exception = Record.Exception(() => JsonSerializer.Deserialize<SkillResponse>(json, AlexaJsonOptions.DefaultOptions));
        exception.Should().BeNull();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void Serialize_ThenDeserialize_RoundTripWorks(SkillRequest originalRequest)
    {
        using var stream = new MemoryStream();
        
        // Serialize
        _serializer.Serialize(originalRequest, stream);
        
        // Reset stream position for reading
        stream.Position = 0;
        
        // Deserialize
        var deserializedRequest = _serializer.Deserialize<SkillRequest>(stream);
        
        deserializedRequest.Should().NotBeNull();
        deserializedRequest!.Request.Type.Should().Be(originalRequest.Request.Type);
        deserializedRequest.Context.System.Application.ApplicationId
            .Should().Be(originalRequest.Context.System.Application.ApplicationId);
    }

    [Fact]
    public void Serialize_WithNullObject_SerializesNull()
    {
        using var stream = new MemoryStream();
        
        _serializer.Serialize<SkillResponse>(null!, stream);
        
        stream.Length.Should().BeGreaterThan(0);
        stream.Position = 0;
        var json = new StreamReader(stream).ReadToEnd();
        json.Should().Be("null");
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void Deserialize_LogsOperationTiming(SkillRequest skillRequest)
    {
        var json = JsonSerializer.Serialize(skillRequest, AlexaJsonOptions.DefaultOptions);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        
        _serializer.Deserialize<SkillRequest>(stream);
        
        // Verify logging was called (timing operation should be logged)
        _testLogger.LogEntries.Should().NotBeEmpty();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void Serialize_LogsOperationTiming(SkillResponse skillResponse)
    {
        using var stream = new MemoryStream();
        
        _serializer.Serialize(skillResponse, stream);
        
        // Verify logging was called (timing operation should be logged)
        _testLogger.LogEntries.Should().NotBeEmpty();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void Deserialize_WithDebugLogging_LogsRawInput(SkillRequest skillRequest)
    {
        _testLogger.MinimumLogLevel = LogLevel.Debug;
        var json = JsonSerializer.Serialize(skillRequest, AlexaJsonOptions.DefaultOptions);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        
        _serializer.Deserialize<SkillRequest>(stream);
        
        _testLogger.HasLogEntry(LogLevel.Debug, "Raw JSON Input").Should().BeTrue();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void Serialize_WithDebugLogging_LogsRawOutput(SkillResponse skillResponse)
    {
        _testLogger.MinimumLogLevel = LogLevel.Debug;
        using var stream = new MemoryStream();
        
        _serializer.Serialize(skillResponse, stream);
        
        _testLogger.HasLogEntry(LogLevel.Debug, "Serialized JSON Output").Should().BeTrue();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void Deserialize_WithLargePayload_HandlesCorrectly(SkillRequest skillRequest)
    {
        var largeRequest = CreateLargeSkillRequest(skillRequest);
        var json = JsonSerializer.Serialize(largeRequest, AlexaJsonOptions.DefaultOptions);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        
        var result = _serializer.Deserialize<SkillRequest>(stream);
        
        result.Should().NotBeNull();
    }

    [Fact]
    public void Serialize_WithLargePayload_HandlesCorrectly()
    {
        var largeResponse = CreateLargeSkillResponse();
        using var stream = new MemoryStream();
        
        var exception = Record.Exception(() => _serializer.Serialize(largeResponse, stream));
        
        exception.Should().BeNull();
        stream.Length.Should().BeGreaterThan(1000); // Ensure it's actually large
    }

    private SkillRequest CreateLargeSkillRequest(SkillRequest request)
    {
        // Add large amounts of data to simulate real-world scenarios
        var largeString = new string('A', 10000);

        request.Session.Attributes = new Dictionary<string, object>
        {
            ["largeString"] = largeString
        };
        
        // This is just for testing - in reality we'd need to properly construct the large data
        return request;
    }

    private SkillResponse CreateLargeSkillResponse()
    {
        var response = new SkillResponse();
        
        // Add large amounts of data for testing
        var largeString = new string('B', 10000);
        response.SessionAttributes = new Dictionary<string, object>
        {
            { "largeString", largeString }
        };
        
        return response;
    }
}