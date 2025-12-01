using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Builders.InteractionModel;

/// <summary>
/// Fluent builder for constructing Alexa interaction models in code.
/// </summary>
public sealed class InteractionModelBuilder
{
    private string _invocationName = string.Empty;
    private string? _version;
    private string? _description;

    private readonly Dictionary<string, IntentBuilder> _intents = [];
    private readonly Dictionary<string, SlotTypeBuilder> _slotTypes = [];
    private NameFreeInteractionBuilder? _nameFreeInteractionBuilder;

    private InteractionModelBuilder()
    {
    }

    /// <summary>
    /// Creates a new <see cref="InteractionModelBuilder"/> instance.
    /// </summary>
    /// <returns>The new <see cref="InteractionModelBuilder"/>.</returns>
    public static InteractionModelBuilder Create() => new();

    /// <summary>
    /// Sets the invocation name for the skill.
    /// </summary>
    /// <param name="invocationName">The invocation name used by Alexa.</param>
    /// <returns>The current <see cref="InteractionModelBuilder"/>.</returns>
    public InteractionModelBuilder WithInvocationName(string invocationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(invocationName);
        _invocationName = invocationName;
        return this;
    }
    /// <summary>
    /// Sets the interaction model version.
    /// </summary>
    /// <param name="version">The interaction model version string.</param>
    /// <returns>The current <see cref="InteractionModelBuilder"/>.</returns>
    public InteractionModelBuilder WithVersion(string version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        _version = version;
        return this;
    }

    /// <summary>
    /// Sets the interaction model description.
    /// </summary>
    /// <param name="description">The description for this interaction model version.</param>
    /// <returns>The current <see cref="InteractionModelBuilder"/>.</returns>
    public InteractionModelBuilder WithDescription(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        _description = description;
        return this;
    }
    
    /// <summary>
    /// Adds or configures an intent in the interaction model.
    /// </summary>
    /// <param name="name">The intent name.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The current <see cref="InteractionModelBuilder"/>.</returns>
    public InteractionModelBuilder AddIntent(
        string name,
        Action<IntentBuilder>? configure = null)
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
    /// Adds or configures a custom slot type in the interaction model.
    /// </summary>
    /// <param name="name">The slot type name.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The current <see cref="InteractionModelBuilder"/>.</returns>
    public InteractionModelBuilder AddSlotType(
        string name,
        Action<SlotTypeBuilder> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(configure);

        if (!_slotTypes.TryGetValue(name, out var builder))
        {
            builder = new SlotTypeBuilder(name);
            _slotTypes[name] = builder;
        }

        configure(builder);
        return this;
    }

    /// <summary>
    /// Configures name-free interaction for the skill.
    /// </summary>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The current <see cref="InteractionModelBuilder"/>.</returns>
    public InteractionModelBuilder WithNameFreeInteraction(Action<NameFreeInteractionBuilder>? configure = null)
    {
        _nameFreeInteractionBuilder ??= new NameFreeInteractionBuilder();
        configure?.Invoke(_nameFreeInteractionBuilder);
        return this;
    }

    /// <summary>
    /// Builds the interaction model definition.
    /// </summary>
    /// <returns>The constructed <see cref="InteractionModelDefinition"/>.</returns>
    public InteractionModelDefinition Build()
    {
        if (string.IsNullOrWhiteSpace(_invocationName))
        {
            throw new InvalidOperationException("Invocation name must be specified.");
        }

        if (string.IsNullOrWhiteSpace(_version))
        {
            throw new InvalidOperationException("Interaction model version must be specified.");
        }

        if (string.IsNullOrWhiteSpace(_description))
        {
            throw new InvalidOperationException("Interaction model description must be specified.");
        }

        var intents = _intents.Values
            .Select(static b => b.Build())
            .ToImmutableArray();

        var slotTypes = _slotTypes.Values
            .Select(static b => b.Build())
            .ToImmutableArray();

        var languageModel = new LanguageModel
        {
            InvocationName = _invocationName,
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
            Version =  _version,
            Description = _description
        };
    }

    /// <summary>
    /// Serializes the interaction model definition to JSON.
    /// </summary>
    /// <param name="options">Optional serializer options.</param>
    /// <returns>The JSON representation of the interaction model.</returns>
    public string ToJson(JsonSerializerOptions? options = null)
    {
        var model = Build();

        options ??= new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        return JsonSerializer.Serialize(model, options);
    }
}
