using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR.Pipeline;

public class RequestInterceptorBehavior : IPipelineBehavior
{
    private readonly IEnumerable<IRequestInterceptor> _requestInterceptors;

    public RequestInterceptorBehavior(IEnumerable<IRequestInterceptor> requestInterceptors) =>
        _requestInterceptors = requestInterceptors;
    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken, RequestHandlerDelegate next)
    {
        foreach (var interceptor in _requestInterceptors)
        {
            await interceptor.Process(input, cancellationToken).ConfigureAwait(false);
        }

        return await next().ConfigureAwait(false);
    }
}