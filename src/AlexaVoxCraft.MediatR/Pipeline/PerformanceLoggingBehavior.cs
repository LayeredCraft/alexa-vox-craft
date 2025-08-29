using System.Diagnostics;
using AlexaVoxCraft.MediatR.Observability;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.MediatR.Pipeline;

/// <summary>
/// Pipeline behavior that adds performance logging, scoped context, and OpenTelemetry instrumentation to Alexa skill request processing.
/// </summary>
public class PerformanceLoggingBehavior : IPipelineBehavior
{
    private readonly ILogger<PerformanceLoggingBehavior> _logger;

    public PerformanceLoggingBehavior(ILogger<PerformanceLoggingBehavior> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken, RequestHandlerDelegate next)
    {
        var request = input.RequestEnvelope.Request;
        var requestType = request.Type;
        var intentName = request is IntentRequest intentRequest ? intentRequest.Intent.Name : null;
        var sessionId = input.RequestEnvelope.Session?.SessionId;
        var userId = input.RequestEnvelope.Session?.User?.UserId;
        var applicationId = input.RequestEnvelope.Context.System.Application.ApplicationId;
        var requestId = request.RequestId;
        var locale = request.Locale ?? "unknown";
        var isNewSession = input.RequestEnvelope.Session?.New ?? false;
        var isColdStart = AlexaVoxCraftTelemetry.IsColdStart();
        
        var hasScreen = HasScreen(input.RequestEnvelope.Context.System.Device?.SupportedInterfaces);
        var dialogState = (request as IntentRequest)?.DialogState;

        using var span = AlexaVoxCraftTelemetry.Source.StartActivity(AlexaSpanNames.Request, ActivityKind.Internal);
        
        span?.SetTag(AlexaSemanticAttributes.RpcSystem, AlexaSemanticValues.RpcSystemAlexa);
        span?.SetTag(AlexaSemanticAttributes.RpcService, applicationId);
        span?.SetTag(AlexaSemanticAttributes.RpcMethod, intentName ?? requestType);
        span?.SetTag(AlexaSemanticAttributes.RequestType, requestType);
        span?.SetTag(AlexaSemanticAttributes.Locale, locale);
        span?.SetTag(AlexaSemanticAttributes.SessionNew, isNewSession ? AlexaSemanticValues.True : AlexaSemanticValues.False);
        span?.SetTag(AlexaSemanticAttributes.DeviceHasScreen, hasScreen ? AlexaSemanticValues.True : AlexaSemanticValues.False);
        span?.SetTag(AlexaSemanticAttributes.RequestId, requestId);
        
        if (isColdStart)
        {
            span?.SetTag(AlexaSemanticAttributes.FaasColdStart, AlexaSemanticValues.True);
        }
        
        if (intentName != null)
        {
            span?.SetTag(AlexaSemanticAttributes.IntentName, intentName);
        }
        
        if (dialogState != null)
        {
            span?.SetTag(AlexaSemanticAttributes.DialogState, dialogState);
        }
        
        if (sessionId != null)
        {
            span?.SetTag(AlexaSemanticAttributes.SessionId, CreateSessionHash(sessionId));
        }

        using var scope = _logger.BeginScope<string, string?, string?, string?, string, string>(
            "RequestType", requestType,
            "IntentName", intentName,
            "SessionId", sessionId,
            "UserId", userId,
            "ApplicationId", applicationId,
            "RequestId", requestId);

        _logger.Debug("Processing Alexa skill request {RequestType} {IntentName}", requestType, intentName);

        using var loggerTimer = _logger.TimeOperation("Skill request processing");
        
        using var latencyTimer = new AlexaVoxCraftTelemetry.TimerScope(AlexaVoxCraftTelemetry.Latency,
            new(AlexaSemanticAttributes.RequestType, requestType),
            new(AlexaSemanticAttributes.IntentName, intentName ?? "none"),
            new(AlexaSemanticAttributes.DeviceHasScreen, hasScreen ? AlexaSemanticValues.True : AlexaSemanticValues.False));

        AlexaVoxCraftTelemetry.Requests.Add(1,
            new(AlexaSemanticAttributes.RequestType, requestType),
            new(AlexaSemanticAttributes.IntentName, intentName ?? "none"),
            new(AlexaSemanticAttributes.Locale, locale),
            new(AlexaSemanticAttributes.DeviceHasScreen, hasScreen ? AlexaSemanticValues.True : AlexaSemanticValues.False),
            new(AlexaSemanticAttributes.ColdStart, isColdStart ? AlexaSemanticValues.True : AlexaSemanticValues.False));

        if (isColdStart)
        {
            AlexaVoxCraftTelemetry.ColdStarts.Add(1);
        }

        try
        {
            ProcessSlotResolutions(span, request);

            var response = await next().ConfigureAwait(false);
            
            ProcessResponseMetrics(span, response, intentName ?? requestType);
            
            span?.SetStatus(ActivityStatusCode.Ok);
            
            _logger.Debug("Successfully processed Alexa skill request {RequestType} {IntentName}", 
                requestType, intentName);

            return response;
        }
        catch (Exception ex)
        {
            span?.AddEvent(new ActivityEvent(AlexaEventNames.Exception,
                DateTimeOffset.UtcNow,
                new ActivityTagsCollection
                {
                    [AlexaSemanticAttributes.ExceptionType] = ex.GetType().FullName!,
                    [AlexaSemanticAttributes.ExceptionMessage] = ex.Message,
                    [AlexaSemanticAttributes.ExceptionStackTrace] = ex.StackTrace ?? ""
                }));
            span?.SetStatus(ActivityStatusCode.Error, ex.Message);
            
            var errorType = ClassifyError(ex);
            AlexaVoxCraftTelemetry.Errors.Add(1,
                new(AlexaSemanticAttributes.ErrorType, errorType),
                new(AlexaSemanticAttributes.IntentName, intentName ?? "none"),
                new(AlexaSemanticAttributes.RequestType, requestType));
            
            _logger.Error(ex, "Failed to process Alexa skill request {RequestType} {IntentName}", 
                requestType, intentName);
            throw;
        }
    }

    private static bool HasScreen(Dictionary<string, object>? supportedInterfaces)
    {
        if (supportedInterfaces == null) return false;
        return supportedInterfaces.ContainsKey("Alexa.Presentation.APL") ||
               supportedInterfaces.ContainsKey(SupportedInterfaces.Display);
    }

    private static string CreateSessionHash(string sessionId)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sessionId));
        return Convert.ToHexString(hash)[..32]; // First 32 chars for brevity while maintaining sufficient entropy
    }

    private static void ProcessSlotResolutions(Activity? span, AlexaVoxCraft.Model.Request.Type.Request request)
    {
        if (request is not IntentRequest intentRequest || intentRequest.Intent?.Slots == null)
            return;

        foreach (var (slotName, slot) in intentRequest.Intent.Slots)
        {
            var status = GetSlotResolutionStatus(slot);
            
            span?.AddEvent(new ActivityEvent(AlexaEventNames.SlotResolution,
                DateTimeOffset.UtcNow,
                new ActivityTagsCollection
                {
                    [AlexaSemanticAttributes.SlotName] = slotName,
                    [AlexaSemanticAttributes.SlotResolutionStatus] = status
                }));

            AlexaVoxCraftTelemetry.SlotResolutions.Add(1,
                new(AlexaSemanticAttributes.SlotName, slotName),
                new(AlexaSemanticAttributes.SlotResolutionStatus, status));
        }
    }

    private static string GetSlotResolutionStatus(Slot slot)
    {
        if (slot.Resolution?.Authorities == null || slot.Resolution.Authorities.Length == 0)
            return AlexaSemanticValues.SlotResolutionNoMatch;

        var authority = slot.Resolution.Authorities[0];
        if (authority.Status is null)
            return AlexaSemanticValues.SlotResolutionError;
            
        return authority.Status.Code switch
        {
            "ER_SUCCESS_MATCH" => AlexaSemanticValues.SlotResolutionMatch,
            "ER_SUCCESS_NO_MATCH" => AlexaSemanticValues.SlotResolutionNoMatch,
            _ => AlexaSemanticValues.SlotResolutionError
        };
    }

    private static void ProcessResponseMetrics(Activity? span, SkillResponse response, string requestIdentifier)
    {
        var speechText = ExtractSpeechText(response.Response.OutputSpeech);
        var repromptText = ExtractSpeechText(response.Response.Reprompt?.OutputSpeech);
        var hasCard = response.Response.Card != null;
        var hasApl = HasAplDirective(response.Response.Directives);
        var shouldEndSession = response.Response.ShouldEndSession ?? true;
        
        if (speechText != null)
        {
            AlexaVoxCraftTelemetry.SpeechCharacters.Record(speechText.Length,
                new(AlexaSemanticAttributes.IntentName, requestIdentifier),
                new(AlexaSemanticAttributes.ResponseHasReprompt, repromptText != null ? AlexaSemanticValues.True : AlexaSemanticValues.False));
        }

        span?.AddEvent(new ActivityEvent(AlexaEventNames.ResponseBuilt,
            DateTimeOffset.UtcNow,
            new ActivityTagsCollection
            {
                [AlexaSemanticAttributes.ResponseHasCard] = hasCard ? AlexaSemanticValues.True : AlexaSemanticValues.False,
                [AlexaSemanticAttributes.ResponseHasApl] = hasApl ? AlexaSemanticValues.True : AlexaSemanticValues.False,
                [AlexaSemanticAttributes.ResponseShouldEndSession] = shouldEndSession ? AlexaSemanticValues.True : AlexaSemanticValues.False,
                [AlexaSemanticAttributes.SpeechCharacters] = speechText?.Length ?? 0,
                [AlexaSemanticAttributes.RepromptCharacters] = repromptText?.Length ?? 0
            }));
    }

    private static string? ExtractSpeechText(IOutputSpeech? outputSpeech)
    {
        return outputSpeech switch
        {
            PlainTextOutputSpeech plainText => plainText.Text,
            SsmlOutputSpeech ssml => ssml.Ssml,
            _ => null
        };
    }

    private static bool HasAplDirective(IList<IDirective>? directives)
    {
        if (directives == null) return false;
        return directives.Any(d => d.Type?.StartsWith("Alexa.Presentation.APL", StringComparison.OrdinalIgnoreCase) == true);
    }

    private static string ClassifyError(Exception ex)
    {
        return ex switch
        {
            ArgumentException => AlexaSemanticValues.ErrorTypeValidation,
            InvalidOperationException => AlexaSemanticValues.ErrorTypeBusiness,
            TimeoutException => AlexaSemanticValues.ErrorTypeTimeout,
            _ => AlexaSemanticValues.ErrorTypeUnhandled
        };
    }
}