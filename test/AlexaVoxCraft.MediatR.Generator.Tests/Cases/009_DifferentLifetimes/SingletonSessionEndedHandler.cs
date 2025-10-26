using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Annotations;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Generated.Function;

[AlexaHandler(Lifetime = ServiceLifetime.Singleton)]
public sealed class SingletonSessionEndedHandler : IRequestHandler<SessionEndedRequest>
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