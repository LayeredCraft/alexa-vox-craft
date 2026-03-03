using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Builders.InteractionModel;

/// <summary>
/// Fluent builder for constructing intent slots.
/// </summary>
public sealed class SlotBuilder
{
    private readonly string _name;
    private readonly string _type;
    private readonly List<string> _samples = [];
    private bool _isRequired;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlotBuilder"/> class.
    /// </summary>
    /// <param name="name">The slot name.</param>
    /// <param name="type">The slot type name.</param>
    public SlotBuilder(string name, string type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(type);

        _name = name;
        _type = type;
    }

    /// <summary>
    /// Marks the slot as required.
    /// </summary>
    /// <returns>The current <see cref="SlotBuilder"/>.</returns>
    public SlotBuilder Required()
    {
        _isRequired = true;
        return this;
    }

    /// <summary>
    /// Marks the slot as optional.
    /// </summary>
    /// <returns>The current <see cref="SlotBuilder"/>.</returns>
    public SlotBuilder Optional()
    {
        _isRequired = false;
        return this;
    }

    /// <summary>
    /// Adds a sample utterance to the slot.
    /// </summary>
    /// <param name="sample">The sample utterance.</param>
    /// <returns>The current <see cref="SlotBuilder"/>.</returns>
    public SlotBuilder WithSample(string sample)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sample);
        _samples.Add(sample);
        return this;
    }

    /// <summary>
    /// Adds multiple sample utterances to the slot.
    /// </summary>
    /// <param name="samples">The sample utterances.</param>
    /// <returns>The current <see cref="SlotBuilder"/>.</returns>
    public SlotBuilder WithSamples(params string[] samples)
    {
        ArgumentNullException.ThrowIfNull(samples);

        foreach (var sample in samples)
        {
            WithSample(sample);
        }

        return this;
    }

    /// <summary>
    /// Builds the slot instance.
    /// </summary>
    /// <returns>The constructed <see cref="IntentSlot"/>.</returns>
    public IntentSlot Build()
        => new()
        {
            Name = _name,
            Type = _type,
            IsRequired = _isRequired,
            Samples = _samples.Count > 0 ? [.. _samples] : null
        };
}