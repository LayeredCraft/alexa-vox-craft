using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace Sample.Generated.Function.Handlers;

public sealed class LaunchHandler : IRequestHandler<LaunchRequest>
{
    private readonly ILogger<LaunchHandler> _logger;

    public LaunchHandler(ILogger<LaunchHandler> logger)
    {
        _logger = logger;
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(input.RequestEnvelope.Request is LaunchRequest);
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var requestType = input.RequestEnvelope.Request.Type;
        var locale = input.RequestEnvelope.Request.Locale;
        
        using var scope = _logger.BeginScope("RequestType", requestType, "Locale", locale);
        
        _logger.Debug("Handling launch request of type {RequestType} in locale {Locale}", requestType, locale);
        
        using var _ = _logger.TimeOperation("Launch response generation");
        
        var response = await input.ResponseBuilder
            .Speak("Hello world from the Launch Request Handler!")
            .WithShouldEndSession(false)
            .GetResponse(cancellationToken);
            
        _logger.Debug("Generated launch response with speech output");
        
        return response;
    }
}