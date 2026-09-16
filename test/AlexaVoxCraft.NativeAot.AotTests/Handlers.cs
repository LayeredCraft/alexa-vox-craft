using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.NativeAot.AotTests;

public sealed class LaunchHandler : IRequestHandler<LaunchRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is LaunchRequest);

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        await input.ResponseBuilder
            .Speak("hello from the native AOT test project")
            .WithShouldEndSession(true)
            .GetResponse(cancellationToken);
}

/// <summary>
/// Dispatch target for the APLSkillRequest regression proof (Issue #190,
/// docs/plans/0004-native-aot-runtime-fixes-and-validation.md Task Group 3). Lets the deserialized
/// UserEventRequest (APLSkillRequest.Request) be routed through the real generator-produced keyed
/// dispatch path, exactly as a real APL-capable skill would, rather than stopping at deserialization.
/// </summary>
public sealed class UserEventHandler : IRequestHandler<UserEventRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is UserEventRequest);

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        await input.ResponseBuilder
            .Speak("hello from the native AOT test project's APL handler")
            .WithShouldEndSession(true)
            .GetResponse(cancellationToken);
}
