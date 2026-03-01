using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class ErrorHandler : IExceptionHandler
{
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(ILogger<ErrorHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken = default) =>
        Task.FromResult(true);

    public async Task<SkillResponse> Handle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken = default)
    {
        _logger.Error(ex, "Error handled: {Message}", ex.Message);

        return await handlerInput.ResponseBuilder
            .Speak("Sorry, I didn't understand what you meant. Please try again.")
            .Reprompt("Sorry, I didn't understand what you meant. Please try again.")
            .GetResponse(cancellationToken);
    }
}