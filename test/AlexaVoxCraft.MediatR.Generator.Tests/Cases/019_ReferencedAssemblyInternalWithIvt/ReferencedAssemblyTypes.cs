using System.Runtime.CompilerServices;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

[assembly: InternalsVisibleTo("GeneratorTests")]

namespace Referenced.WithIvt;

public sealed class Marker;

public sealed class PublicReferencedLaunchHandler : IRequestHandler<LaunchRequest>
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

internal sealed class InternalReferencedLaunchHandler : IRequestHandler<LaunchRequest>
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
