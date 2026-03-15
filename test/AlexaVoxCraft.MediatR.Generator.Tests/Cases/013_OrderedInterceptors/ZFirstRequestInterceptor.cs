using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Annotations;
using AlexaVoxCraft.MediatR.Pipeline;

namespace Sample.Generated.Function;

[AlexaHandler(Order = 1)]
public sealed class ZFirstRequestInterceptor : IRequestInterceptor
{
    public Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}