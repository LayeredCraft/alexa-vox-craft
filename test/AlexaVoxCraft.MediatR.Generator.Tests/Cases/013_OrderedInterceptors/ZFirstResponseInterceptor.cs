using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Annotations;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;

namespace Sample.Generated.Function;

[AlexaHandler(Order = 1)]
public sealed class ZFirstResponseInterceptor : IResponseInterceptor
{
    public Task Process(IHandlerInput input, SkillResponse response, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}