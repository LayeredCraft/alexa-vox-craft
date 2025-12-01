using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;
using Amazon.Lambda.Core;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AlexaVoxCraft.Lambda.Serialization;

/// <summary>
/// Provides JSON serialization and deserialization for Alexa skill requests and responses in AWS Lambda.
/// </summary>
public sealed class AlexaLambdaSerializer : ILambdaSerializer
{
    private readonly ILogger<AlexaLambdaSerializer> _logger;
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlexaLambdaSerializer"/> class.
    /// </summary>
    /// <param name="logger">Logger for serialization diagnostics.</param>
    /// <param name="jsonSerializerOptions">JSON serialization options. Uses <see cref="AlexaJsonOptions.DefaultOptions"/> if null.</param>
    public AlexaLambdaSerializer(ILogger<AlexaLambdaSerializer> logger, JsonSerializerOptions? jsonSerializerOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = jsonSerializerOptions ?? AlexaJsonOptions.DefaultOptions;
    }

    /// <summary>
    /// Deserializes a Lambda request from a stream.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="requestStream">The stream containing the request data.</param>
    /// <returns>The deserialized request object.</returns>
    public T? Deserialize<T>(Stream requestStream)
    {
        using var _ = _logger.TimeOperation("Request deserialization");
        
        var streamLength = requestStream.Length;
        var typeName = typeof(T).Name;


        var obj = JsonSerializer.Deserialize<T>(requestStream, _options);
            
            
        _logger.Debug("Deserialized {RequestType} payload ({PayloadSize} bytes)", typeName, streamLength);
            
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.Debug("📥 Raw JSON Input: {@RawRequest}", obj);
        }

        return obj;
    }

    /// <summary>
    /// Serializes a Lambda response to a stream.
    /// </summary>
    /// <typeparam name="T">The type to serialize.</typeparam>
    /// <param name="response">The response object to serialize.</param>
    /// <param name="responseStream">The stream to write the serialized data to.</param>
    public void Serialize<T>(T response, Stream responseStream)
    {
        using var _ = _logger.TimeOperation("Response serialization");
        
        var typeName = typeof(T).Name;
        
        
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.Debug("📤 Serialized JSON Output: {@RawResponse}", response);
        }

        var initialPosition = responseStream.Position;
        JsonSerializer.Serialize(responseStream, response, _options);
        var serializedSize = responseStream.Position - initialPosition;
            
            
            
        _logger.Debug("Serialized {ResponseType} payload ({PayloadSize} bytes)", typeName, serializedSize);
    }
}