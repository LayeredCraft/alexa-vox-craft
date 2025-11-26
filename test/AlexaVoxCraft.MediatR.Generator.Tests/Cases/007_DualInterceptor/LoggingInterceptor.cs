using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;

namespace Sample.Generated.Function;

public sealed class LoggingInterceptor :
    IRequestInterceptor,
    IResponseInterceptor
{
    public Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Process(IHandlerInput input, SkillResponse response, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}