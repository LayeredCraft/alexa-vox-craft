using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;
using Amazon.Lambda.Core;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AlexaVoxCraft.MediatR.Lambda.Serialization;

public sealed class AlexaLambdaSerializer : ILambdaSerializer
{
    private readonly ILogger<AlexaLambdaSerializer> _logger;
    private readonly JsonSerializerOptions _options;

    public AlexaLambdaSerializer(ILogger<AlexaLambdaSerializer> logger, JsonSerializerOptions? jsonSerializerOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = jsonSerializerOptions ?? AlexaJsonOptions.DefaultOptions;
    }

    public T? Deserialize<T>(Stream requestStream)
    {
        using var _ = _logger.TimeOperation("Request deserialization");
        
        var streamLength = requestStream.Length;
        var obj = JsonSerializer.Deserialize<T>(requestStream, _options);
        
        _logger.Debug("Deserialized {RequestType} payload ({PayloadSize} bytes)", typeof(T).Name, streamLength);
        
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.Debug("📥 Raw JSON Input: {@RawRequest}", obj);
        }

        return obj;
    }

    public void Serialize<T>(T response, Stream responseStream)
    {
        using var _ = _logger.TimeOperation("Response serialization");
        
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.Debug("📤 Serialized JSON Output: {@RawResponse}", response);
        }

        var initialPosition = responseStream.Position;
        JsonSerializer.Serialize(responseStream, response, _options);
        var serializedSize = responseStream.Position - initialPosition;
        
        _logger.Debug("Serialized {ResponseType} payload ({PayloadSize} bytes)", typeof(T).Name, serializedSize);
    }
}