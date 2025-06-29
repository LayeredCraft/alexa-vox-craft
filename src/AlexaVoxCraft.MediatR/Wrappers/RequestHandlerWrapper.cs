using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.Wrappers;

public abstract class RequestHandlerWrapper : HandlerBase
{
    public abstract Task<SkillResponse> Handle(SkillRequest request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

public class RequestHandlerWrapperImpl<TRequestType> : RequestHandlerWrapper where TRequestType : Request
{
    public override Task<SkillResponse> Handle(SkillRequest request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var handlerInput = serviceProvider.GetRequiredService<IHandlerInput>();

        async Task<SkillResponse> Handler()
        {
            var handlers = GetHandlers<IRequestHandler<TRequestType>>(serviceProvider);
            foreach (var handler in handlers)
            {
                if (await handler.CanHandle(handlerInput, cancellationToken))
                {
                    return await handler.Handle(handlerInput, cancellationToken);
                }
            }

            var defaultHandler = serviceProvider.GetService<IDefaultRequestHandler>();
            if (defaultHandler is not null && await defaultHandler.CanHandle(handlerInput, cancellationToken))
            {
                return await defaultHandler.Handle(handlerInput, cancellationToken);
            }

            throw new InvalidOperationException(
                $"Handler was not found for request of type {typeof(IRequestHandler<TRequestType>)}. Register your handlers with the container.");
        }


        return serviceProvider
            .GetServices<IPipelineBehavior>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate)Handler,
                (next, pipeline) => () => pipeline.Handle(handlerInput, cancellationToken, next))();
    }
}