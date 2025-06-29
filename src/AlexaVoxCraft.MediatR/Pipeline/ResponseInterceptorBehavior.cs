using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR.Pipeline;

public class ResponseInterceptorBehavior : IPipelineBehavior
{
    private readonly IEnumerable<IResponseInterceptor> _responseInterceptors;

    public ResponseInterceptorBehavior(IEnumerable<IResponseInterceptor> responseInterceptors) =>
        _responseInterceptors = responseInterceptors;
    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken, RequestHandlerDelegate next)
    {
        var response = await next().ConfigureAwait(false);

        foreach (var interceptor in _responseInterceptors)
        {
            await interceptor.Process(input, response, cancellationToken);
        }

        return response;
    }
}