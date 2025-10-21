using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

namespace Sample.Generated.Function.Handlers;

public abstract class BaseHandler<TRequest> : IRequestHandler<TRequest> where TRequest : Request
{
    public virtual Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is TRequest);

    public abstract Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default);
}