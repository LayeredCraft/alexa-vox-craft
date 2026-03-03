using System.Text.Json;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Attributes.Persistence;

/// <summary>
/// Defines the contract for loading and saving persistent skill attributes
/// backed by an external store such as DynamoDB or S3.
/// </summary>
public interface IPersistenceAdapter
{
    /// <summary>
    /// Loads the persistent attributes for the given skill request from the backing store.
    /// </summary>
    /// <param name="requestEnvelope">The current skill request used to resolve the storage key.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A dictionary of attribute key/value pairs, or an empty dictionary if none exist.</returns>
    Task<IDictionary<string, JsonElement>> GetAttributes(SkillRequest requestEnvelope,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the given attributes for the skill request to the backing store.
    /// </summary>
    /// <param name="requestEnvelope">The current skill request used to resolve the storage key.</param>
    /// <param name="attributes">The attributes to persist.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    Task SaveAttribute(SkillRequest requestEnvelope, IDictionary<string, JsonElement> attributes,
        CancellationToken cancellationToken = default);
}