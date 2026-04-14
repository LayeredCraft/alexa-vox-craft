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
        /// <param name="configureHttpClientBuilder">
        /// Optional callback for additional <see cref="IHttpClientBuilder"/> customization,
        /// such as adding delegating handlers for logging, request capture, resiliency policies, or testing.
        /// </param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSmapiDeveloperClient(configuration);
        /// // Or with custom section name:
        /// services.AddSmapiDeveloperClient(configuration, "AlexaSkillManagement");
        ///
        /// // Or with HttpClient customization:
        /// services.AddSmapiDeveloperClient(
        ///     configuration,
        ///     configureHttpClientBuilder: builder =>
        ///     {
        ///         builder.AddHttpMessageHandler&lt;RequestCaptureHandler&gt;();
        ///     });
        /// </code>
        /// </example>
        public IServiceCollection AddSmapiDeveloperClient(IConfiguration configuration,
            string sectionName = "SmapiClient",
            Action<IHttpClientBuilder>? configureHttpClientBuilder = null)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            return services.AddSmapiDeveloperClient(options => configuration.GetSection(sectionName).Bind(options),
                configureHttpClientBuilder);
        }

        /// <summary>
        /// Registers SMAPI developer client services for use in CI/CD pipelines and external tooling.
        /// Uses Login with Amazon (LWA) refresh token authentication with credentials from the Amazon Developer Console.
        /// </summary>
        /// <param name="optionsAction">The action to configure SMAPI developer credentials (ClientId, ClientSecret, RefreshToken).</param>
        /// <param name="configureHttpClientBuilder">
        /// Optional callback for additional <see cref="IHttpClientBuilder"/> customization,
        /// such as adding delegating handlers for logging, request capture, resiliency policies, or testing.
        /// </param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSmapiDeveloperClient(options =>
        /// {
        ///     options.ClientId = "amzn1.application-oa2-client.xxx";
        ///     options.ClientSecret = "your-secret";
        ///     options.RefreshToken = "Atzr|xxx";
        /// });
        ///
        /// services.AddSmapiDeveloperClient(
        ///     options =>
        ///     {
        ///         options.ClientId = "amzn1.application-oa2-client.xxx";
        ///         options.ClientSecret = "your-secret";
        ///         options.RefreshToken = "Atzr|xxx";
        ///     },
        ///     builder =>
        ///     {
        ///         builder.AddHttpMessageHandler&lt;RequestCaptureHandler&gt;();
        ///     });
        /// </code>
        /// </example>
        public IServiceCollection AddSmapiDeveloperClient(Action<SmapiDeveloperAccessTokenOptions> optionsAction,
            Action<IHttpClientBuilder>? configureHttpClientBuilder = null)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(optionsAction);

            services.Configure(optionsAction);
            var httpClientBuilder = services
                .AddHttpClient<IAlexaInteractionModelClient, AlexaInteractionModelClient>(client =>
                {
                    client.BaseAddress = new Uri("https://api.amazonalexa.com/");
                })
                .AddAuthorizationForwarding();

            configureHttpClientBuilder?.Invoke(httpClientBuilder);

            services.AddScoped<IAccessTokenProvider, SmapiDeveloperAccessTokenProvider>();
            return services;
        }

        /// <summary>
        /// Registers the Alexa Skill Invocation client for invoking skills during testing.
        /// Uses Login with Amazon (LWA) refresh token authentication with credentials from configuration.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="sectionName">
        /// The configuration section name containing SMAPI credentials.
        /// Defaults to <c>InvocationClient</c>.
        /// </param>
        /// <param name="configureHttpClientBuilder">
        /// Optional callback for additional <see cref="IHttpClientBuilder"/> customization,
        /// such as adding delegating handlers for logging, request capture, resiliency policies, or testing.
        /// </param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSkillInvocationClient(configuration);
        ///
        /// // Or with a custom section name:
        /// services.AddSkillInvocationClient(configuration, "AlexaSkillInvocation");
        ///
        /// // Or with HttpClient customization:
        /// services.AddSkillInvocationClient(
        ///     configuration,
        ///     configureHttpClientBuilder: builder =>
        ///     {
        ///         builder.AddHttpMessageHandler&lt;RequestCaptureHandler&gt;();
        ///     });
        /// </code>
        /// </example>
        public IServiceCollection AddSkillInvocationClient(IConfiguration configuration,
            string sectionName = "InvocationClient",
            Action<IHttpClientBuilder>? configureHttpClientBuilder = null)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            return services.AddSkillInvocationClient(options => configuration.GetSection(sectionName).Bind(options),
                configureHttpClientBuilder);
        }

        /// <summary>
        /// Registers the Alexa Skill Invocation client for invoking skills during testing.
        /// Uses Login with Amazon (LWA) refresh token authentication with credentials from the Amazon Developer Console.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="optionsAction">
        /// The action to configure SMAPI developer credentials
        /// (<c>ClientId</c>, <c>ClientSecret</c>, <c>RefreshToken</c>).
        /// </param>
        /// <param name="configureHttpClientBuilder">
        /// Optional callback for additional <see cref="IHttpClientBuilder"/> customization,
        /// such as adding delegating handlers for logging, request capture, resiliency policies, or testing.
        /// </param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSkillInvocationClient(options =>
        /// {
        ///     options.ClientId = "amzn1.application-oa2-client.xxx";
        ///     options.ClientSecret = "your-secret";
        ///     options.RefreshToken = "Atzr|xxx";
        /// });
        ///
        /// services.AddSkillInvocationClient(
        ///     options =>
        ///     {
        ///         options.ClientId = "amzn1.application-oa2-client.xxx";
        ///         options.ClientSecret = "your-secret";
        ///         options.RefreshToken = "Atzr|xxx";
        ///     },
        ///     builder =>
        ///     {
        ///         builder.AddHttpMessageHandler&lt;RequestCaptureHandler&gt;();
        ///     });
        /// </code>
        /// </example>
        public IServiceCollection AddSkillInvocationClient(Action<SmapiDeveloperAccessTokenOptions> optionsAction,
            Action<IHttpClientBuilder>? configureHttpClientBuilder = null)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(optionsAction);

            services.Configure(optionsAction);
            var httpClientBuilder = services
                .AddHttpClient<IAlexaSkillInvocationClient, AlexaSkillInvocationClient>(client =>
                {
                    client.BaseAddress = new Uri("https://api.amazonalexa.com/");
                })
                .AddAuthorizationForwarding();

            configureHttpClientBuilder?.Invoke(httpClientBuilder);

            services.AddScoped<IAccessTokenProvider, SmapiDeveloperAccessTokenProvider>();
            return services;
        }
    }
}
