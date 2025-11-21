using AlexaVoxCraft.Smapi.Models.InteractionModel;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.Smapi.Clients;

/// <summary>
/// Client for managing Alexa skill interaction models via the Skill Management API (SMAPI).
/// </summary>
public sealed class AlexaInteractionModelClient : BaseClient, IAlexaInteractionModelClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlexaInteractionModelClient"/> class.
    /// </summary>
    /// <param name="client">The configured HTTP client with base address and authentication.</param>
    /// <param name="logger">The logger instance.</param>
    public AlexaInteractionModelClient(HttpClient client, ILogger<AlexaInteractionModelClient> logger) : base(client, logger)
    {
    }

    /// <inheritdoc />
    public async Task<InteractionModelDefinition?> GetAsync(string skillId, string stage, string locale,
        CancellationToken ct)
    {
        return await GetAsync<InteractionModelDefinition>(
            new Uri($"/v1/skills/{skillId}/stages/{stage}/interactionModel/locales/{locale}", UriKind.Relative),
            null, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(string skillId, string stage, string locale, InteractionModelDefinition model,
        CancellationToken ct)
    {
        await PutAsync(
                new Uri($"/v1/skills/{skillId}/stages/{stage}/interactionModel/locales/{locale}", UriKind.Relative),
                model, null, ct)
            .ConfigureAwait(false);
    }
}