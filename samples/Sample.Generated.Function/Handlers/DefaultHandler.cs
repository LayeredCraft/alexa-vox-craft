using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace Sample.Generated.Function.Handlers;

public class DefaultHandler : IDefaultRequestHandler
{
    private readonly ILogger<DefaultHandler> _logger;

    public DefaultHandler(ILogger<DefaultHandler> logger)
    {
        _logger = logger;
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var requestType = input.RequestEnvelope.Request.Type;
        var locale = input.RequestEnvelope.Request.Locale;
        
        using var scope = _logger.BeginScope("RequestType", requestType, "Locale", locale);
        
        _logger.Debug("Handling default request of type {RequestType} in locale {Locale}", requestType, locale);
        
        using var _ = _logger.TimeOperation("Default response generation");
        
        var response = await input.ResponseBuilder
            .Speak("Hello world from the Default Handler!")
            .WithShouldEndSession(true)
            .GetResponse(cancellationToken);
            
        _logger.Debug("Generated default response with speech output");
        
        return response;
    }
}