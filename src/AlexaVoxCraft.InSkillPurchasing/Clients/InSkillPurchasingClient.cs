using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using AlexaVoxCraft.Http.Clients;
using AlexaVoxCraft.Http.Serialization;
using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.InSkillPurchasing.Serialization;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.InSkillPurchasing.Clients;

/// <summary>
/// HTTP client implementation for the Alexa In-Skill Purchasing API.
/// </summary>
public sealed class InSkillPurchasingClient : BaseClient, IInSkillPurchasingClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InSkillPurchasingClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured with the ISP base address.</param>
    /// <param name="logger">The logger instance.</param>
    public InSkillPurchasingClient(HttpClient httpClient, ILogger<InSkillPurchasingClient> logger) : base(httpClient,
        logger, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            TypeInfoResolver = JsonTypeInfoResolver.Combine(InSkillPurchasingClientModelContext.Default, DelegatingModelTypeInfoResolver.Instance),
        })
    {
    }

    /// <inheritdoc />
    public async Task<ProductResponse?> GetProductsAsync(ProductFilter? filter = null,
        CancellationToken cancellationToken = default) => await GetAsync<ProductResponse>(
        new Uri($"{InSkillPurchasingEndpoints.Products}{filter.ToQueryString()}", UriKind.Relative),
        null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Product?> GetProductAsync(string productId, CancellationToken cancellationToken = default) =>
        await GetAsync<Product>(
            new Uri($"{InSkillPurchasingEndpoints.Products}/{productId}", UriKind.Relative),
            null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public Task<PurchasingEnabled?> GetPurchasingEnabledAsync(CancellationToken cancellationToken = default) =>
        GetAsync<PurchasingEnabled>(
            new Uri(InSkillPurchasingEndpoints.VoicePurchasing, UriKind.Relative),
            null, cancellationToken);

    /// <inheritdoc />
    public Task<TransactionResponse?> GetTransactionsAsync(TransactionFilter? filter = null,
        CancellationToken cancellationToken = default) => GetAsync<TransactionResponse>(
        new Uri($"{InSkillPurchasingEndpoints.Transactions}{filter.ToQueryString()}", UriKind.Relative),
        null, cancellationToken);

    private static class InSkillPurchasingEndpoints
    {
        private const string Base = "v1/users/~current/skills/~current";

        public const string Products =
            $"{Base}/inSkillProducts";

        public const string VoicePurchasing =
            $"{Base}/settings/voicePurchasing.enabled";

        public const string Transactions =
            $"{Base}/inSkillProductsTransactions";
    }
}