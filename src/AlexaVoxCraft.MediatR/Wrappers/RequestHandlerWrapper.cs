using System.Diagnostics;
using AlexaVoxCraft.MediatR.Observability;
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
            var executionOrder = 0;
            
            foreach (var handler in handlers)
            {
                var handlerType = handler.GetType().Name;
                
                // Track handler resolution attempt
                AlexaVoxCraftTelemetry.HandlerResolutionAttempts.Add(1,
                    new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerType, handlerType),
                    new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerExecutionOrder, executionOrder));

                // Instrument CanHandle() call
                using var resolutionSpan = AlexaVoxCraftTelemetry.Source.StartActivity(AlexaSpanNames.HandlerResolution, ActivityKind.Internal);
                using var resolutionTimer = AlexaVoxCraftTelemetry.TimeHandlerResolution();
                
                resolutionSpan?.SetTag(AlexaSemanticAttributes.HandlerType, handlerType);
                resolutionSpan?.SetTag(AlexaSemanticAttributes.HandlerExecutionOrder, executionOrder);
                resolutionSpan?.SetTag(AlexaSemanticAttributes.HandlerIsDefault, false);

                bool canHandle;
                try
                {
                    canHandle = await handler.CanHandle(handlerInput, cancellationToken);
                    resolutionSpan?.SetStatus(ActivityStatusCode.Ok);
                }
                catch (Exception ex)
                {
                    resolutionSpan?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    throw;
                }
                
                resolutionSpan?.SetTag(AlexaSemanticAttributes.HandlerCanHandle, canHandle);
                
                if (canHandle)
                {
                    // Track handler execution
                    AlexaVoxCraftTelemetry.HandlerExecutions.Add(1,
                        new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerType, handlerType),
                        new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerIsDefault, false));

                    // Instrument Handle() call
                    using var executionSpan = AlexaVoxCraftTelemetry.Source.StartActivity(AlexaSpanNames.Handler, ActivityKind.Internal);
                    using var executionTimer = AlexaVoxCraftTelemetry.TimeHandlerExecution(handlerType, false);
                    
                    executionSpan?.SetTag(AlexaSemanticAttributes.HandlerType, handlerType);
                    executionSpan?.SetTag(AlexaSemanticAttributes.HandlerIsDefault, false);

                    try
                    {
                        var response = await handler.Handle(handlerInput, cancellationToken);
                        executionSpan?.SetStatus(ActivityStatusCode.Ok);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        executionSpan?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        throw;
                    }
                }
                
                executionOrder++;
            }

            // Handle default handler
            var defaultHandler = serviceProvider.GetService<IDefaultRequestHandler>();
            if (defaultHandler is not null)
            {
                var defaultHandlerType = defaultHandler.GetType().Name;
                
                // Track default handler resolution attempt
                AlexaVoxCraftTelemetry.HandlerResolutionAttempts.Add(1,
                    new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerType, defaultHandlerType),
                    new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerExecutionOrder, executionOrder),
                    new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerIsDefault, true));

                // Instrument default handler CanHandle() call
                using var defaultResolutionSpan = AlexaVoxCraftTelemetry.Source.StartActivity(AlexaSpanNames.HandlerResolution, ActivityKind.Internal);
                using var defaultResolutionTimer = AlexaVoxCraftTelemetry.TimeHandlerResolution();
                
                defaultResolutionSpan?.SetTag(AlexaSemanticAttributes.HandlerType, defaultHandlerType);
                defaultResolutionSpan?.SetTag(AlexaSemanticAttributes.HandlerExecutionOrder, executionOrder);
                defaultResolutionSpan?.SetTag(AlexaSemanticAttributes.HandlerIsDefault, true);

                bool canHandleDefault;
                try
                {
                    canHandleDefault = await defaultHandler.CanHandle(handlerInput, cancellationToken);
                    defaultResolutionSpan?.SetStatus(ActivityStatusCode.Ok);
                }
                catch (Exception ex)
                {
                    defaultResolutionSpan?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    throw;
                }
                
                defaultResolutionSpan?.SetTag(AlexaSemanticAttributes.HandlerCanHandle, canHandleDefault);
                
                if (canHandleDefault)
                {
                    // Track default handler execution
                    AlexaVoxCraftTelemetry.HandlerExecutions.Add(1,
                        new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerType, defaultHandlerType),
                        new KeyValuePair<string, object?>(AlexaSemanticAttributes.HandlerIsDefault, true));

                    // Instrument default handler Handle() call
                    using var defaultExecutionSpan = AlexaVoxCraftTelemetry.Source.StartActivity(AlexaSpanNames.Handler, ActivityKind.Internal);
                    using var defaultExecutionTimer = AlexaVoxCraftTelemetry.TimeHandlerExecution(defaultHandlerType, true);
                    
                    defaultExecutionSpan?.SetTag(AlexaSemanticAttributes.HandlerType, defaultHandlerType);
                    defaultExecutionSpan?.SetTag(AlexaSemanticAttributes.HandlerIsDefault, true);

                    try
                    {
                        var response = await defaultHandler.Handle(handlerInput, cancellationToken);
                        defaultExecutionSpan?.SetStatus(ActivityStatusCode.Ok);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        defaultExecutionSpan?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        throw;
                    }
                }
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