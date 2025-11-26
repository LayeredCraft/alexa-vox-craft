using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Pipeline;

namespace Sample.Generated.Function;

public sealed class RequestInterceptor : IRequestInterceptor
{
    public Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}