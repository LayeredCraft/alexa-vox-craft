using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR.Pipeline;

public class RequestExceptionProcessBehavior : IPipelineBehavior
{
    private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;

    public RequestExceptionProcessBehavior(IEnumerable<IExceptionHandler> exceptionHandlers) =>
        _exceptionHandlers = exceptionHandlers;
    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken, RequestHandlerDelegate next)
    {
        try
        {
            return await next().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            foreach (var exceptionHandler in _exceptionHandlers)
            {
                if (await exceptionHandler.CanHandle(input, e, cancellationToken))
                {
                    return await exceptionHandler.Handle(input, e, cancellationToken);
                }                
            }
            throw;
        }
    }
}