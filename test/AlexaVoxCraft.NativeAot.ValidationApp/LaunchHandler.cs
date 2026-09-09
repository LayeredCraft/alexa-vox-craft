using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.NativeAot.ValidationApp;

public sealed class LaunchHandler : IRequestHandler<LaunchRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is LaunchRequest);

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        await input.ResponseBuilder
            .Speak("hello from the native AOT validation app")
            .WithShouldEndSession(true)
            .GetResponse(cancellationToken);
}
