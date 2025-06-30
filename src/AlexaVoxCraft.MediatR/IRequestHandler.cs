using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR;

public interface IRequestHandler
{
    Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default);

    Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default);
}

public interface IDefaultRequestHandler : IRequestHandler
{
    public new Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(true);
}

public interface IRequestHandler<TRequestType> : IRequestHandler where TRequestType : Request
{
}
