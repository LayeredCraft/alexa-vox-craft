using AlexaVoxCraft.Lambda.Serialization;
using AlexaVoxCraft.Model.Serialization;
using Amazon.Lambda.Core;
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
        /// Registers Alexa skill hosting services including serialization and Lambda context access.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
        public IServiceCollection AddAlexaSkillHost()
        {
            services.AddSingleton(AlexaJsonOptions.DefaultOptions);
            services.AddSingleton<ILambdaSerializer, AlexaLambdaSerializer>();
            services.AddLambdaHostContextAccessor();
            return services;
        }
    }
}