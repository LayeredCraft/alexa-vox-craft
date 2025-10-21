using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace Sample.Generated.Function.Handlers;

public sealed class ErrorHandler : IExceptionHandler
{
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(ILogger<ErrorHandler> logger) => _logger = logger;

    public Task<bool>
        CanHandle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken = default) =>
        Task.FromResult(true);

    public async Task<SkillResponse> Handle(IHandlerInput handlerInput, Exception ex,
        CancellationToken cancellationToken = default)
    {
        var requestType = handlerInput.RequestEnvelope.Request.Type;
        var locale = handlerInput.RequestEnvelope.Request.Locale;
        
        using var scope = _logger.BeginScope("RequestType", requestType, "Locale", locale);
        
        _logger.Debug(ex,"Handling launch request of type {RequestType} in locale {Locale}", requestType, locale);
        
        using var _ = _logger.TimeOperation("Launch response generation");
        
        var response = await handlerInput.ResponseBuilder
            .Speak("Hello world from the Error Handler!")
            .WithShouldEndSession(false)
            .GetResponse(cancellationToken);
            
        _logger.Debug("Generated launch response with speech output");
        
        return response;

    }
}