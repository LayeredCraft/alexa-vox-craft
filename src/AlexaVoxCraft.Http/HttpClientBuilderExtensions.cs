using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlexaVoxCraft.Http;

/// <summary>
/// Extension methods for <see cref="IHttpClientBuilder"/> to configure authentication handlers.
/// </summary>
public static class HttpClientBuilderExtensions
{
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