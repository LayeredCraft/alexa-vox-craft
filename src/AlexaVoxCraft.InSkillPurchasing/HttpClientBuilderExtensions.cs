using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlexaVoxCraft.InSkillPurchasing;

/// <summary>
/// Extension methods for <see cref="IHttpClientBuilder"/> to configure In-Skill Purchasing handlers.
/// </summary>
public static class HttpClientBuilderExtensions
{
    extension(IHttpClientBuilder builder)
    {
        /// <summary>
        /// Adds Locale to the HTTP client pipeline.
        /// </summary>
        /// <returns>The HTTP client builder for chaining.</returns>
        public IHttpClientBuilder AddLocale()
        {
            builder.Services.TryAddTransient<LocaleHandler>();
            builder.AddHttpMessageHandler<LocaleHandler>();
            return builder;
        }
    }
}