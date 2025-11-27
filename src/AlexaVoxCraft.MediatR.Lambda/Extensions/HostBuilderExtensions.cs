using AlexaVoxCraft.Lambda.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AlexaVoxCraft.MediatR.Lambda.Extensions;

public static class HostBuilderExtensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseHandler<THandler, TRequest, TResponse>()
            where THandler : ILambdaHandler<TRequest, TResponse>
        {
            builder.UseHandler(CreateDelegate<THandler, TRequest, TResponse>);
            return builder;
        }

        private void UseHandler<TRequest, TResponse>(Func<IServiceProvider, HandlerDelegate<TRequest, TResponse>> handlerFactory)
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(handlerFactory);
            });
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