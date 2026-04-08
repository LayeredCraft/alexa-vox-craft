using AlexaVoxCraft.Http;
using AlexaVoxCraft.InSkillPurchasing.Auth;
using AlexaVoxCraft.InSkillPurchasing.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.InSkillPurchasing;

/// <summary>
/// Extension methods for registering In-Skill Purchasing services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the <see cref="IInSkillPurchasingClient"/> and its dependencies, including bearer token
        /// authentication and locale forwarding, configured against the Alexa API endpoint.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddInSkillPurchasing()
        {
            services.AddHttpClient<IInSkillPurchasingClient, InSkillPurchasingClient>(client =>
                {
                    client.BaseAddress = new Uri("https://api.amazonalexa.com/");
                })
                .AddLocale()
                .AddAuthorizationForwarding();
            services.AddScoped<IAccessTokenProvider, AlexaRequestAccessTokenProvider>();
            return services;
        }
    }
}