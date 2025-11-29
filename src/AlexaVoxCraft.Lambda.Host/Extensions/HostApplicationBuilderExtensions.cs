using AlexaVoxCraft.Lambda.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AlexaVoxCraft.Lambda.Host.Extensions;

/// <summary>
/// Extension methods for configuring Lambda handlers with <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Registers a Lambda handler implementation to process requests.
        /// </summary>
        /// <typeparam name="THandler">The handler implementation type.</typeparam>
        /// <typeparam name="TRequest">The request type to process.</typeparam>
        /// <typeparam name="TResponse">The response type to return.</typeparam>
        /// <returns>The <see cref="IHostApplicationBuilder"/> for chaining.</returns>
        public IHostApplicationBuilder UseHandler<THandler, TRequest, TResponse>()
            where THandler : ILambdaHandler<TRequest, TResponse>
        {
            builder.UseHandler(CreateDelegate<THandler, TRequest, TResponse>);
            return builder;
        }

        /// <summary>
        /// Registers a Lambda handler using a factory function.
        /// </summary>
        /// <typeparam name="TRequest">The request type to process.</typeparam>
        /// <typeparam name="TResponse">The response type to return.</typeparam>
        /// <param name="factory">Factory function to create the handler delegate.</param>
        /// <returns>The <see cref="IHostApplicationBuilder"/> for chaining.</returns>
        IHostApplicationBuilder UseHandler<TRequest, TResponse>(
            Func<IServiceProvider, HandlerDelegate<TRequest, TResponse>> factory)
        {
            builder.Services.AddScoped(factory);
            return builder;
        }
    }
    private static HandlerDelegate<TRequest, TResponse>
        CreateDelegate<THandler, TRequest, TResponse>(IServiceProvider requestedServices)
        where THandler : ILambdaHandler<TRequest, TResponse>
    {
        var handler = ActivatorUtilities.CreateInstance<THandler>(requestedServices);

        return async (request, context, cancellationToken) =>
        {
            var response = await handler.HandleAsync(request, context, cancellationToken);
            return response;
        };
    }
}