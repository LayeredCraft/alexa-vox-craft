using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.MediatR.Pipeline;

/// <summary>
/// Pipeline behavior that adds performance logging and scoped context to Alexa skill request processing.
/// </summary>
public class PerformanceLoggingBehavior : IPipelineBehavior
{
    private readonly ILogger<PerformanceLoggingBehavior> _logger;

    public PerformanceLoggingBehavior(ILogger<PerformanceLoggingBehavior> logger)
    {
        _logger = logger;
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

        using var scope = _logger.BeginScope<string, string?, string?, string?, string, string>(
            "RequestType", requestType,
            "IntentName", intentName,
            "SessionId", sessionId,
            "UserId", userId,
            "ApplicationId", applicationId,
            "RequestId", requestId);

        _logger.Debug("Processing Alexa skill request {RequestType} {IntentName}", requestType, intentName);

        using var timer = _logger.TimeOperation("Skill request processing");

        try
        {
            var response = await next().ConfigureAwait(false);
            
            _logger.Debug("Successfully processed Alexa skill request {RequestType} {IntentName}", 
                requestType, intentName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to process Alexa skill request {RequestType} {IntentName}", 
                requestType, intentName);
            throw;
        }
    }
}