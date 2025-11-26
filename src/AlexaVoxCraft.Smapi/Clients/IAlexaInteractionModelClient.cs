using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Clients;

/// <summary>
/// Client for managing Alexa skill interaction models via the Skill Management API (SMAPI).
/// </summary>
public interface IAlexaInteractionModelClient
{
    /// <summary>
    /// Retrieves the interaction model for a specific skill, stage, and locale.
    /// </summary>
    /// <param name="skillId">The Alexa skill identifier.</param>
    /// <param name="stage">The skill stage (e.g., "development", "live").</param>
    /// <param name="locale">The locale code (e.g., "en-US", "en-GB").</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The interaction model definition, or null if not found.</returns>
    Task<InteractionModelDefinition?> GetAsync(string skillId, string stage, string locale, CancellationToken ct);

    /// <summary>
    /// Updates the interaction model for a specific skill, stage, and locale.
    /// </summary>
    /// <param name="skillId">The Alexa skill identifier.</param>
    /// <param name="stage">The skill stage (e.g., "development", "live").</param>
    /// <param name="locale">The locale code (e.g., "en-US", "en-GB").</param>
    /// <param name="model">The interaction model definition to upload.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(string skillId, string stage, string locale, InteractionModelDefinition model, CancellationToken ct);
}