using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

namespace Sample.Generated.InaccessibleTypes;

public sealed class PrivateNestedHandlers
{
    private sealed class PrivateNestedLaunchHandler : IRequestHandler<LaunchRequest>
    {
        public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
