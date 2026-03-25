using System.Collections.Immutable;
using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Builders.InteractionModel;

/// <summary>
/// Fluent builder for constructing Alexa interaction models across multiple locales.
/// Shared schema elements (intent names, slot definitions, slot type names) are defined once.
/// Locale-specific text (invocation name, sample utterances, slot values) is defined per locale,
/// with unoverridden values falling back to the default locale.
/// </summary>
public sealed class MultiLocaleInteractionModelBuilder
{
    private string? _version;
    private string? _description;

    private readonly Dictionary<string, IntentBuilder> _intents = [];
    private readonly Dictionary<string, SlotTypeBuilder> _slotTypes = [];
    private NameFreeInteractionBuilder? _nameFreeInteractionBuilder;

    private string? _defaultLocale;
    private LocaleOverrideBuilder? _defaultOverrides;
    private readonly Dictionary<string, LocaleOverrideBuilder> _locales = [];

    private MultiLocaleInteractionModelBuilder()
    {
    }

    /// <summary>
    /// Creates a new <see cref="MultiLocaleInteractionModelBuilder"/> instance.
    /// </summary>
    public static MultiLocaleInteractionModelBuilder Create() => new();

    /// <summary>
    /// Sets the interaction model version.
    /// </summary>
    /// <param name="version">The interaction model version string.</param>
    /// <returns>The current <see cref="MultiLocaleInteractionModelBuilder"/>.</returns>
    public MultiLocaleInteractionModelBuilder WithVersion(string version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        _version = version;
        return this;
    }

    /// <summary>
    /// Sets the interaction model description.
    /// </summary>
    /// <param name="description">The description for this interaction model version.</param>
    /// <returns>The current <see cref="MultiLocaleInteractionModelBuilder"/>.</returns>
    public MultiLocaleInteractionModelBuilder WithDescription(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        _description = description;
        return this;
    }

    /// <summary>
    /// Adds or configures an intent in the shared schema.
    /// Samples set here act as a global fallback used when neither the locale nor the default locale provides samples for this intent.
    /// Prefer setting samples via <see cref="WithDefaultLocale"/> and <see cref="ForLocale"/> for locale-specific control.
    /// </summary>
    /// <param name="name">The intent name.</param>
    /// <param name="configure">Optional configuration action for slots and schema-level fallback samples.</param>
    /// <returns>The current <see cref="MultiLocaleInteractionModelBuilder"/>.</returns>
    public MultiLocaleInteractionModelBuilder AddIntent(string name, Action<IntentBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_intents.TryGetValue(name, out var builder))
        {
            builder = new IntentBuilder(name);
            _intents[name] = builder;
        }

        configure?.Invoke(builder);
        return this;
    }

    /// <summary>
    /// Registers a custom slot type in the shared schema.
    /// Slot values are locale-specific; provide them per locale via <see cref="LocaleOverrideBuilder.WithSlotValues"/>.
    /// </summary>
    /// <param name="name">The slot type name.</param>
    /// <returns>The current <see cref="MultiLocaleInteractionModelBuilder"/>.</returns>
    public MultiLocaleInteractionModelBuilder AddSlotType(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_slotTypes.ContainsKey(name))
            _slotTypes[name] = new SlotTypeBuilder(name);

        return this;
    }

    /// <summary>
    /// Configures name-free interaction for the skill. Applied to all locales.
    /// </summary>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The current <see cref="MultiLocaleInteractionModelBuilder"/>.</returns>
    public MultiLocaleInteractionModelBuilder WithNameFreeInteraction(
        Action<NameFreeInteractionBuilder>? configure = null)
    {
        _nameFreeInteractionBuilder ??= new NameFreeInteractionBuilder();
        configure?.Invoke(_nameFreeInteractionBuilder);
        return this;
    }

    /// <summary>
    /// Sets the default locale and its text content. The default locale is included in <see cref="BuildAll"/> output
    /// and provides fallback values for all other locales registered via <see cref="ForLocale"/>.
    /// </summary>
    /// <param name="locale">The locale code (e.g., "en-US").</param>
    /// <param name="configure">The configuration action for the default locale's text content.</param>
    /// <returns>The current <see cref="MultiLocaleInteractionModelBuilder"/>.</returns>
    public MultiLocaleInteractionModelBuilder WithDefaultLocale(string locale, Action<LocaleOverrideBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locale);
        ArgumentNullException.ThrowIfNull(configure);

        if (_defaultLocale is not null && _defaultLocale != locale)
            throw new InvalidOperationException($"Default locale is already set to '{_defaultLocale}'.");

        _defaultLocale = locale;

        if (_defaultOverrides is null && _locales.TryGetValue(locale, out var promoted))
        {
            _defaultOverrides = promoted;
            _locales.Remove(locale);
        }

        _defaultOverrides ??= new LocaleOverrideBuilder(_intents, _slotTypes);
        configure(_defaultOverrides);
        return this;
    }

    /// <summary>
    /// Registers an additional locale. Any text not overridden here falls back to the default locale.
    /// Calling this method multiple times for the same locale merges into the same override builder.
    /// </summary>
    /// <param name="locale">The locale code (e.g., "en-GB").</param>
    /// <param name="configure">Optional configuration action. Omit to inherit everything from the default locale.</param>
    /// <returns>The current <see cref="MultiLocaleInteractionModelBuilder"/>.</returns>
    public MultiLocaleInteractionModelBuilder ForLocale(string locale, Action<LocaleOverrideBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locale);

        if (locale == _defaultLocale)
        {
            _defaultOverrides ??= new LocaleOverrideBuilder(_intents, _slotTypes);
            configure?.Invoke(_defaultOverrides);
            return this;
        }

        if (!_locales.TryGetValue(locale, out var builder))
        {
            builder = new LocaleOverrideBuilder(_intents, _slotTypes);
            _locales[locale] = builder;
        }

        configure?.Invoke(builder);
        return this;
    }

    /// <summary>
    /// Builds an interaction model definition for each registered locale, applying default locale fallbacks
    /// for any text not explicitly overridden.
    /// </summary>
    /// <returns>A list of <see cref="LocalizedInteractionModel"/> instances, default locale first.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no default locale has been set.</exception>
    public IReadOnlyList<LocalizedInteractionModel> BuildAll()
    {
        if (_defaultLocale is null || _defaultOverrides is null)
            throw new InvalidOperationException("A default locale must be specified using WithDefaultLocale.");

        if (string.IsNullOrWhiteSpace(_version))
            throw new InvalidOperationException("Interaction model version must be specified.");

        if (string.IsNullOrWhiteSpace(_description))
            throw new InvalidOperationException("Interaction model description must be specified.");

        if (string.IsNullOrWhiteSpace(_defaultOverrides.InvocationName))
            throw new InvalidOperationException("Invocation name must be specified in the default locale.");

        var result = new List<LocalizedInteractionModel>
        {
            new(_defaultLocale, BuildDefinitionForLocale(_defaultOverrides))
        };

        foreach (var (locale, overrides) in _locales)
        {
            if (locale == _defaultLocale)
                continue;

            result.Add(new(locale, BuildDefinitionForLocale(overrides)));
        }

        return result.AsReadOnly();
    }

    private InteractionModelDefinition BuildDefinitionForLocale(LocaleOverrideBuilder overrides)
    {
        var invocationName = overrides.InvocationName ?? _defaultOverrides!.InvocationName!;

        var intents = _intents.Values.Select(intentBuilder =>
        {
            var intentSamples = overrides.GetIntentSamples(intentBuilder.Name)
                ?? _defaultOverrides!.GetIntentSamples(intentBuilder.Name)
                ?? intentBuilder.Samples;

            var slotSampleOverrides = intentBuilder.SlotNames.ToDictionary(
                slotName => slotName,
                slotName => (IReadOnlyList<string>?)(overrides.GetSlotSamples(intentBuilder.Name, slotName)
                    ?? _defaultOverrides!.GetSlotSamples(intentBuilder.Name, slotName)));

            return intentBuilder.BuildWithSamples(intentSamples, slotSampleOverrides);
        }).ToImmutableArray();

        var slotTypes = _slotTypes.Select(kvp =>
        {
            var configAction = overrides.GetSlotTypeConfig(kvp.Key)
                ?? _defaultOverrides!.GetSlotTypeConfig(kvp.Key);

            var builder = new SlotTypeBuilder(kvp.Key);
            configAction?.Invoke(builder);
            return builder.Build();
        }).ToImmutableArray();

        var languageModel = new LanguageModel
        {
            InvocationName = invocationName,
            Intents = intents,
            Types = slotTypes
        };

        var body = new InteractionModelBody
        {
            LanguageModel = languageModel,
            NameFreeInteraction = _nameFreeInteractionBuilder?.Build()
        };

        return new InteractionModelDefinition
        {
            InteractionModel = body,
            Version = _version!,
            Description = _description!
        };
    }
}
