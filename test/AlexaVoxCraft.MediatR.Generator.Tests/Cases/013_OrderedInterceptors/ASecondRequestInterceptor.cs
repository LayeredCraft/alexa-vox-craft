using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Annotations;
using AlexaVoxCraft.MediatR.Pipeline;

namespace Sample.Generated.Function;

[AlexaHandler(Order = 2)]
public sealed class ASecondRequestInterceptor : IRequestInterceptor
{
    public Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}