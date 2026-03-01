using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class HelpHandler : IRequestHandler<IntentRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest { Intent.Name: BuiltInIntent.Help });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        await input.ResponseBuilder
            .Speak("To hear a random fact, you could say, 'Tell me a fact' or you can ask for a specific " +
                   "category you have purchased, for example, say 'Tell me a science fact'. " +
                   "To know what else you can buy, say, 'What can I buy?'. So, what can I help you with?")
            .Reprompt("I didn't catch that. What can I help you with?")
            .GetResponse(cancellationToken);
}