using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Annotations;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;

namespace Sample.Generated.Function;

[AlexaHandler(Order = 2)]
public sealed class ASecondResponseInterceptor : IResponseInterceptor
{
    public Task Process(IHandlerInput input, SkillResponse response, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}