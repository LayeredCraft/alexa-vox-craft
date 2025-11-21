using AlexaVoxCraft.Smapi.Auth;
using AlexaVoxCraft.Smapi.Clients;
using AlexaVoxCraft.Smapi.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlexaVoxCraft.Smapi;

/// <summary>
/// Extension methods for configuring SMAPI client services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers SMAPI client services with configuration from the specified configuration section.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="sectionName">The configuration section name containing SMAPI credentials. Defaults to "SmapiClient".</param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSmapiClient(configuration);
        /// // Or with custom section name:
        /// services.AddSmapiClient(configuration, "AlexaSkillManagement");
        /// </code>
        /// </example>
        public IServiceCollection AddSmapiClient(IConfiguration configuration,
            string sectionName = "SmapiClient")
        {
            return services.AddSmapiClient(options => configuration.GetSection(sectionName).Bind(options));
        }

        /// <summary>
        /// Registers SMAPI client services with configuration via an action delegate.
        /// </summary>
        /// <param name="optionsAction">The action to configure SMAPI developer credentials.</param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSmapiClient(options =>
        /// {
        ///     options.ClientId = "amzn1.application-oa2-client.xxx";
        ///     options.ClientSecret = "your-secret";
        ///     options.RefreshToken = "Atzr|xxx";
        /// });
        /// </code>
        /// </example>
        public IServiceCollection AddSmapiClient(Action<SmapiDeveloperAccessTokenOptions> optionsAction)
        {
            services.Configure(optionsAction);
            services.AddHttpClient<IAlexaInteractionModelClient, AlexaInteractionModelClient>(client =>
                {
                    client.BaseAddress = new Uri("https://api.amazonalexa.com/");
                })
                .AddAuthorizationForwarding();
            services.AddSingleton<IAccessTokenProvider, SmapiDeveloperAccessTokenProvider>();
            return services;
        }
    }

    extension(IHttpClientBuilder builder)
    {
        /// <summary>
        /// Adds bearer token authentication to the HTTP client pipeline.
        /// </summary>
        /// <returns>The HTTP client builder for chaining.</returns>
        public IHttpClientBuilder AddAuthorizationForwarding()
        {
            builder.Services.TryAddTransient<BearerTokenHandler>();
            builder.AddHttpMessageHandler<BearerTokenHandler>();
            return builder;
        }
    }
}