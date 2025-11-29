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
        /// Registers Alexa skill hosting services including JSON serialization, Lambda context access, and skill request factory.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
        /// <remarks>
        /// Registers the following services:
        /// <list type="bullet">
        /// <item><description>Alexa JSON serialization options with custom converters</description></item>
        /// <item><description>Lambda serializer for request/response processing</description></item>
        /// <item><description>Lambda host context accessor for accessing execution context</description></item>
        /// <item><description>Skill request factory for retrieving the current skill request</description></item>
        /// </list>
        /// </remarks>
        public IServiceCollection AddAlexaSkillHost()
        {
            services.AddSingleton(AlexaJsonOptions.DefaultOptions);
            services.AddSingleton<ILambdaSerializer, AlexaLambdaSerializer>();
            services.AddLambdaHostContextAccessor();
            services.AddScoped<SkillRequestFactory>(sp =>
                () => sp.GetRequiredService<ILambdaHostContextAccessor>().LambdaHostContext
                    ?.GetRequiredEvent<SkillRequest>());
            return services;
        }
    }
}