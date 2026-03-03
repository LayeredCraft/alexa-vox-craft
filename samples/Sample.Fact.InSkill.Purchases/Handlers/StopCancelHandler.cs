using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using Sample.Fact.InSkill.Purchases.Helpers;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class StopCancelHandler : IRequestHandler<IntentRequest>
{
    private readonly ILogger<StopCancelHandler> _logger;

    public StopCancelHandler(ILogger<StopCancelHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest
        {
            Intent.Name: BuiltInIntent.Stop or BuiltInIntent.Cancel
        });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(StopCancelHandler), nameof(Handle));

        return await input.ResponseBuilder
            .Speak(FactHelpers.GetRandomGoodbye())
            .GetResponse(cancellationToken);
    }
}