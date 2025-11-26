using System.Collections.Immutable;
using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Builders.InteractionModel;

/// <summary>
/// Fluent builder for constructing Alexa intents.
/// </summary>
public sealed class IntentBuilder
{
    private readonly string _name;
    private readonly List<string> _samples = [];
    private readonly List<SlotBuilder> _slots = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="IntentBuilder"/> class.
    /// </summary>
    /// <param name="name">The intent name.</param>
    public IntentBuilder(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _name = name;
    }

    /// <summary>
    /// Adds a sample utterance to the intent.
    /// </summary>
    /// <param name="sample">The sample utterance.</param>
    /// <returns>The current <see cref="IntentBuilder"/>.</returns>
    public IntentBuilder WithSample(string sample)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sample);
        _samples.Add(sample);
        return this;
    }

    /// <summary>
    /// Adds multiple sample utterances to the intent.
    /// </summary>
    /// <param name="samples">The sample utterances.</param>
    /// <returns>The current <see cref="IntentBuilder"/>.</returns>
    public IntentBuilder WithSamples(params string[] samples)
    {
        ArgumentNullException.ThrowIfNull(samples);

        foreach (var sample in samples)
        {
            WithSample(sample);
        }

        return this;
    }

    /// <summary>
    /// Adds or configures a slot on the intent.
    /// </summary>
    /// <param name="name">The slot name.</param>
    /// <param name="type">The slot type name.</param>
    /// <param name="configure">Optional configuration action for the slot.</param>
    /// <returns>The current <see cref="IntentBuilder"/>.</returns>
    public IntentBuilder WithSlot(
        string name,
        string type,
        Action<SlotBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(type);

        var builder = new SlotBuilder(name, type);
        configure?.Invoke(builder);
        _slots.Add(builder);
        return this;
    }

    /// <summary>
    /// Builds the intent instance.
    /// </summary>
    /// <returns>The constructed <see cref="Intent"/>.</returns>
    public Intent Build()
    {
        var slots = _slots
            .Select(static s => s.Build())
            .ToImmutableArray();

        return new Intent
        {
            Name = _name,
            Samples = [.._samples],
            Slots = slots
        };
    }
}
