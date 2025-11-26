using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;

namespace Sample.Generated.Function;

public sealed class TriviaExceptionHandler : IExceptionHandler
{
    public Task<bool> CanHandle(IHandlerInput handlerInput, Exception exception, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SkillResponse> Handle(IHandlerInput handlerInput, Exception exception, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}