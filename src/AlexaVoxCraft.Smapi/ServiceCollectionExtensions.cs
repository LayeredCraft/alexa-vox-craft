using AlexaVoxCraft.Http;
using AlexaVoxCraft.Smapi.Auth;
using AlexaVoxCraft.Smapi.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.Smapi;

/// <summary>
/// Extension methods for configuring SMAPI client services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers SMAPI developer client services for use in CI/CD pipelines and external tooling.
        /// Uses Login with Amazon (LWA) refresh token authentication with credentials from the Amazon Developer Console.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="sectionName">The configuration section name containing SMAPI credentials. Defaults to "SmapiClient".</param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSmapiDeveloperClient(configuration);
        /// // Or with custom section name:
        /// services.AddSmapiDeveloperClient(configuration, "AlexaSkillManagement");
        /// </code>
        /// </example>
        public IServiceCollection AddSmapiDeveloperClient(IConfiguration configuration,
            string sectionName = "SmapiClient")
        {
            return services.AddSmapiDeveloperClient(options => configuration.GetSection(sectionName).Bind(options));
        }

        /// <summary>
        /// Registers SMAPI developer client services for use in CI/CD pipelines and external tooling.
        /// Uses Login with Amazon (LWA) refresh token authentication with credentials from the Amazon Developer Console.
        /// </summary>
        /// <param name="optionsAction">The action to configure SMAPI developer credentials (ClientId, ClientSecret, RefreshToken).</param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSmapiDeveloperClient(options =>
        /// {
        ///     options.ClientId = "amzn1.application-oa2-client.xxx";
        ///     options.ClientSecret = "your-secret";
        ///     options.RefreshToken = "Atzr|xxx";
        /// });
        /// </code>
        /// </example>
        public IServiceCollection AddSmapiDeveloperClient(Action<SmapiDeveloperAccessTokenOptions> optionsAction)
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

        /// <summary>
        /// Registers the Alexa Skill Invocation client for invoking skills during testing.
        /// Uses Login with Amazon (LWA) refresh token authentication with credentials from the Amazon Developer Console.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="sectionName">The configuration section name containing SMAPI credentials. Defaults to "InvocationClient".</param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSkillInvocationClient(configuration);
        /// // Or with custom section name:
        /// services.AddSkillInvocationClient(configuration, "AlexaSkillInvocation");
        /// </code>
        /// </example>
        public IServiceCollection AddSkillInvocationClient(IConfiguration configuration,
            string sectionName = "InvocationClient")
        {
            return services.AddSkillInvocationClient(options => configuration.GetSection(sectionName).Bind(options));
        }

        /// <summary>
        /// Registers the Alexa Skill Invocation client for invoking skills during testing.
        /// Uses Login with Amazon (LWA) refresh token authentication with credentials from the Amazon Developer Console.
        /// </summary>
        /// <param name="optionsAction">The action to configure SMAPI developer credentials (ClientId, ClientSecret, RefreshToken).</param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSkillInvocationClient(options =>
        /// {
        ///     options.ClientId = "amzn1.application-oa2-client.xxx";
        ///     options.ClientSecret = "your-secret";
        ///     options.RefreshToken = "Atzr|xxx";
        /// });
        /// </code>
        /// </example>
        public IServiceCollection AddSkillInvocationClient(Action<SmapiDeveloperAccessTokenOptions> optionsAction)
        {
            services.Configure(optionsAction);
            services.AddHttpClient<IAlexaSkillInvocationClient, AlexaSkillInvocationClient>(client =>
                {
                    client.BaseAddress = new Uri("https://api.amazonalexa.com/");
                })
                .AddAuthorizationForwarding();
            services.AddSingleton<IAccessTokenProvider, SmapiDeveloperAccessTokenProvider>();
            return services;
        }
    }
}