using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.Wrappers;

public abstract class HandlerBase
{
    protected static IEnumerable<THandler> GetHandlers<THandler>(IServiceProvider serviceProvider)
    {
        IEnumerable<THandler> handlers;

        try
        {
            handlers = serviceProvider.GetServices<THandler>();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Error constructing handler for request of type {typeof(THandler)}. Register your handlers with the container.", e);
        }

        if (handlers is null)
        {
            throw new InvalidOperationException($"Handler was not found for request of type {typeof(THandler)}. Register your handlers with the container.");
        }
        
        return handlers;
    }
}