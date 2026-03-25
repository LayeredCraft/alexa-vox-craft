namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Pairs a locale code with its <see cref="InteractionModelDefinition"/>.
/// </summary>
/// <param name="Locale">The locale code (e.g., "en-US", "en-GB").</param>
/// <param name="Definition">The interaction model definition for this locale.</param>
public sealed record LocalizedInteractionModel(string Locale, InteractionModelDefinition Definition);