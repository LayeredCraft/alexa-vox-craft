using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.Lambda.Serialization;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Serialization;
using Amazon.Lambda.Core;
using AwsLambda.Host.Core;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.Lambda.Host.Extensions;

/// <summary>
/// Extension methods for configuring Alexa skill services with <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers Alexa skill hosting services and handler delegate.
        /// </summary>
        /// <typeparam name="THandler">The handler implementation type.</typeparam>
        /// <typeparam name="TRequest">The request type to process.</typeparam>
        /// <typeparam name="TResponse">The response type to return.</typeparam>
        /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
        /// <remarks>
        /// Registers the following services:
        /// <list type="bullet">
        /// <item><description>Alexa JSON serialization options with custom converters</description></item>
        /// <item><description>Lambda serializer for request/response processing</description></item>
        /// <item><description>Lambda host context accessor for accessing execution context</description></item>
        /// <item><description>Skill request factory for retrieving the current skill request</description></item>
        /// <item><description>Handler delegate for processing skill requests</description></item>
        /// </list>
        /// After calling this method, use <c>app.MapHandler(AlexaHandler.Invoke&lt;TRequest, TResponse&gt;)</c>
        /// on the built Lambda application to complete the handler configuration.
        /// </remarks>
        public IServiceCollection AddAlexaSkillHost<THandler, TRequest, TResponse>()
            where THandler : ILambdaHandler<TRequest, TResponse>
        {
            services.AddSingleton(AlexaJsonOptions.DefaultOptions);
            services.AddSingleton<ILambdaSerializer, AlexaLambdaSerializer>();
            services.AddLambdaHostContextAccessor();
            services.AddScoped<SkillRequestFactory>(sp =>
                () => sp.GetRequiredService<ILambdaHostContextAccessor>().LambdaHostContext
                    ?.GetRequiredEvent<SkillRequest>());

            services.AddScoped<HandlerDelegate<TRequest, TResponse>>(sp =>
            {
                var handler = ActivatorUtilities.CreateInstance<THandler>(sp);
                return async (request, context, cancellationToken) =>
                {
                    var response = await handler.HandleAsync(request, context, cancellationToken);
                    return response;
                };
            });

            return services;
        }
    }
}