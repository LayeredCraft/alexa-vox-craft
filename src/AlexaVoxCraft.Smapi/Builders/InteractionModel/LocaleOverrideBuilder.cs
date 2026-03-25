namespace AlexaVoxCraft.Smapi.Builders.InteractionModel;

/// <summary>
/// Builder for locale-specific text overrides within a <see cref="MultiLocaleInteractionModelBuilder"/>.
/// Any value not overridden falls back to the default locale's value at build time.
/// </summary>
public sealed class LocaleOverrideBuilder
{
    private readonly IReadOnlyDictionary<string, IntentBuilder> _schemaIntents;
    private readonly IReadOnlyDictionary<string, SlotTypeBuilder> _schemaSlotTypes;

    private string? _invocationName;
    private readonly Dictionary<string, IReadOnlyList<string>> _intentSamples = [];
    private readonly Dictionary<string, Dictionary<string, IReadOnlyList<string>>> _slotSamples = [];
    private readonly Dictionary<string, Action<SlotTypeBuilder>> _slotValueConfigs = [];

    internal LocaleOverrideBuilder(
        IReadOnlyDictionary<string, IntentBuilder> schemaIntents,
        IReadOnlyDictionary<string, SlotTypeBuilder> schemaSlotTypes)
    {
        _schemaIntents = schemaIntents;
        _schemaSlotTypes = schemaSlotTypes;
    }

    /// <summary>
    /// Sets the invocation name for this locale.
    /// </summary>
    /// <param name="name">The locale-specific invocation name.</param>
    /// <returns>The current <see cref="LocaleOverrideBuilder"/>.</returns>
    public LocaleOverrideBuilder WithInvocationName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _invocationName = name;
        return this;
    }

    /// <summary>
    /// Sets the sample utterances for the specified intent in this locale.
    /// Replaces any previously set samples for this intent in this locale.
    /// </summary>
    /// <param name="intentName">The intent name. Must be defined in the schema.</param>
    /// <param name="samples">The locale-specific sample utterances.</param>
    /// <returns>The current <see cref="LocaleOverrideBuilder"/>.</returns>
    public LocaleOverrideBuilder WithIntentSamples(string intentName, params string[] samples)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(intentName);
        ArgumentNullException.ThrowIfNull(samples);

        if (!_schemaIntents.ContainsKey(intentName))
            throw new InvalidOperationException($"Intent '{intentName}' is not defined in the interaction model schema.");

        foreach (var sample in samples)
            ArgumentException.ThrowIfNullOrWhiteSpace(sample, nameof(samples));

        _intentSamples[intentName] = [..samples];
        return this;
    }

    /// <summary>
    /// Sets the sample utterances for the specified slot in this locale.
    /// Replaces any previously set samples for this slot in this locale.
    /// </summary>
    /// <param name="intentName">The intent name. Must be defined in the schema.</param>
    /// <param name="slotName">The slot name. Must be defined on the intent.</param>
    /// <param name="samples">The locale-specific slot sample utterances.</param>
    /// <returns>The current <see cref="LocaleOverrideBuilder"/>.</returns>
    public LocaleOverrideBuilder WithSlotSamples(string intentName, string slotName, params string[] samples)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(intentName);
        ArgumentException.ThrowIfNullOrWhiteSpace(slotName);
        ArgumentNullException.ThrowIfNull(samples);

        if (!_schemaIntents.TryGetValue(intentName, out var intentBuilder))
            throw new InvalidOperationException($"Intent '{intentName}' is not defined in the interaction model schema.");

        if (!intentBuilder.SlotNames.Contains(slotName))
            throw new InvalidOperationException($"Slot '{slotName}' is not defined on intent '{intentName}'.");

        foreach (var sample in samples)
            ArgumentException.ThrowIfNullOrWhiteSpace(sample, nameof(samples));

        if (!_slotSamples.TryGetValue(intentName, out var slots))
        {
            slots = [];
            _slotSamples[intentName] = slots;
        }

        slots[slotName] = [..samples];
        return this;
    }

    /// <summary>
    /// Configures the values for the specified slot type in this locale.
    /// Replaces any previously set configuration for this slot type in this locale.
    /// </summary>
    /// <param name="slotTypeName">The slot type name. Must be defined in the schema.</param>
    /// <param name="configure">The configuration action for the slot type values.</param>
    /// <returns>The current <see cref="LocaleOverrideBuilder"/>.</returns>
    public LocaleOverrideBuilder WithSlotValues(string slotTypeName, Action<SlotTypeBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slotTypeName);
        ArgumentNullException.ThrowIfNull(configure);

        if (!_schemaSlotTypes.ContainsKey(slotTypeName))
            throw new InvalidOperationException($"Slot type '{slotTypeName}' is not defined in the interaction model schema.");

        _slotValueConfigs[slotTypeName] = configure;
        return this;
    }

    internal string? InvocationName => _invocationName;

    internal IReadOnlyList<string>? GetIntentSamples(string intentName) =>
        _intentSamples.TryGetValue(intentName, out var samples) ? samples : null;

    internal IReadOnlyList<string>? GetSlotSamples(string intentName, string slotName) =>
        _slotSamples.TryGetValue(intentName, out var slots) && slots.TryGetValue(slotName, out var samples)
            ? samples
            : null;

    internal Action<SlotTypeBuilder>? GetSlotTypeConfig(string slotTypeName) =>
        _slotValueConfigs.TryGetValue(slotTypeName, out var config) ? config : null;
}