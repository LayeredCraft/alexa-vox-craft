using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace Sample.Generated.Function.Handlers;

public class AnswerHandler : IRequestHandler<IntentRequest>, IRequestHandler<UserEventRequest>
{
    private readonly ILogger<AnswerHandler> _logger;

    public AnswerHandler(ILogger<AnswerHandler> logger)
    {
        _logger = logger;
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(input.RequestEnvelope.Request is UserEventRequest or IntentRequest
        {
            Intent.Name: "Answer" or "DontKnow" or BuiltInIntent.Next
        });
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var requestType = input.RequestEnvelope.Request.Type;
        var locale = input.RequestEnvelope.Request.Locale;
        
        using var scope = _logger.BeginScope("RequestType", requestType, "Locale", locale);
        
        _logger.Debug("Handling answer request of type {RequestType} in locale {Locale}", requestType, locale);
        
        using var _ = _logger.TimeOperation("Answer response generation");
        
        var response = await input.ResponseBuilder
            .Speak("Hello world from the Answer Request Handler!")
            .WithShouldEndSession(false)
            .GetResponse(cancellationToken);
            
        _logger.Debug("Generated answer response with speech output");
        
        return response;
    }
}