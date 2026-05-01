using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Pipeline;

namespace Sample.Generated.InternalTypes;

internal sealed class InternalRequestInterceptor : IRequestInterceptor
{
    public Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
