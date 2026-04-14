using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Annotations;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

namespace Sample.Generated.Function;

[AlexaHandler(Order = 20)]
public sealed class SourceOrderedLaunchHandler : IRequestHandler<LaunchRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
