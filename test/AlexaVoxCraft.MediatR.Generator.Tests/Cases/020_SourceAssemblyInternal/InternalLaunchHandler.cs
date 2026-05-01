using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

namespace Sample.Generated.InternalTypes;

internal sealed class InternalLaunchHandler : IRequestHandler<LaunchRequest>
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
