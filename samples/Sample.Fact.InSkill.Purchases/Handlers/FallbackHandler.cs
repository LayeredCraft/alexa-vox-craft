using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class FallbackHandler : IRequestHandler<IntentRequest>
{
    private readonly ILogger<FallbackHandler> _logger;

    public FallbackHandler(ILogger<FallbackHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest { Intent.Name: BuiltInIntent.Fallback });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(FallbackHandler), nameof(Handle));

        return await input.ResponseBuilder
            .Speak("Sorry, I didn't understand what you meant. Please try again.")
            .Reprompt("Sorry, I didn't understand what you meant. Please try again.")
            .GetResponse(cancellationToken);
    }
}