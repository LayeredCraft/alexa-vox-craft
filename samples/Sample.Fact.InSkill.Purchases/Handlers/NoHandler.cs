using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using Sample.Fact.InSkill.Purchases.Helpers;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class NoHandler : IRequestHandler<IntentRequest>
{
    private readonly ILogger<NoHandler> _logger;

    public NoHandler(ILogger<NoHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest { Intent.Name: BuiltInIntent.No });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(NoHandler), nameof(Handle));

        return await input.ResponseBuilder
            .Speak(FactHelpers.GetRandomGoodbye())
            .GetResponse(cancellationToken);
    }
}