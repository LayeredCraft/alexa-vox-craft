using System.Diagnostics;
using System.Text.Json;
using AlexaVoxCraft.MediatR.Observability;
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
        using var span = AlexaVoxCraftTelemetry.Source.StartActivity(AlexaSpanNames.SerializationRequest, ActivityKind.Internal);
        using var timer = AlexaVoxCraftTelemetry.TimeRequestSerialization();
        using var _ = _logger.TimeOperation("Request deserialization");
        
        var streamLength = requestStream.Length;
        var typeName = typeof(T).Name;
        
        span?.SetTag(AlexaSemanticAttributes.SerializationDirection, AlexaSemanticValues.SerializationDirectionRequest);
        span?.SetTag(AlexaSemanticAttributes.PayloadSize, streamLength);
        span?.SetTag(AlexaSemanticAttributes.CodeNamespace, typeof(T).Namespace ?? "");
        span?.SetTag(AlexaSemanticAttributes.CodeFunction, typeName);
        
        try
        {
            var obj = JsonSerializer.Deserialize<T>(requestStream, _options);
            
            AlexaVoxCraftTelemetry.PayloadSize.Record(streamLength,
                new(AlexaSemanticAttributes.SerializationDirection, AlexaSemanticValues.SerializationDirectionRequest),
                new(AlexaSemanticAttributes.CodeFunction, typeName));
            
            _logger.Debug("Deserialized {RequestType} payload ({PayloadSize} bytes)", typeName, streamLength);
            
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.Debug("📥 Raw JSON Input: {@RawRequest}", obj);
            }

            span?.SetStatus(ActivityStatusCode.Ok);
            return obj;
        }
        catch (Exception ex)
        {
            span?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    public void Serialize<T>(T response, Stream responseStream)
    {
        using var span = AlexaVoxCraftTelemetry.Source.StartActivity(AlexaSpanNames.SerializationResponse, ActivityKind.Internal);
        using var timer = AlexaVoxCraftTelemetry.TimeResponseSerialization();
        using var _ = _logger.TimeOperation("Response serialization");
        
        var typeName = typeof(T).Name;
        
        span?.SetTag(AlexaSemanticAttributes.SerializationDirection, AlexaSemanticValues.SerializationDirectionResponse);
        span?.SetTag(AlexaSemanticAttributes.CodeNamespace, typeof(T).Namespace ?? "");
        span?.SetTag(AlexaSemanticAttributes.CodeFunction, typeName);
        
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.Debug("📤 Serialized JSON Output: {@RawResponse}", response);
        }

        try
        {
            var initialPosition = responseStream.Position;
            JsonSerializer.Serialize(responseStream, response, _options);
            var serializedSize = responseStream.Position - initialPosition;
            
            span?.SetTag(AlexaSemanticAttributes.PayloadSize, serializedSize);
            
            AlexaVoxCraftTelemetry.PayloadSize.Record(serializedSize,
                new(AlexaSemanticAttributes.SerializationDirection, AlexaSemanticValues.SerializationDirectionResponse),
                new(AlexaSemanticAttributes.CodeFunction, typeName));
            
            _logger.Debug("Serialized {ResponseType} payload ({PayloadSize} bytes)", typeName, serializedSize);
            
            span?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            span?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}