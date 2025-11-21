using System.Collections.Immutable;
using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Builders.InteractionModel;

/// <summary>
/// Fluent builder for constructing slot type values.
/// </summary>
public sealed class SlotTypeValueBuilder
{
    private readonly string _value;
    private readonly List<string> _synonyms = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SlotTypeValueBuilder"/> class.
    /// </summary>
    /// <param name="value">The primary value.</param>
    public SlotTypeValueBuilder(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        _value = value;
    }

    /// <summary>
    /// Adds a synonym for the value.
    /// </summary>
    /// <param name="synonym">The synonym to add.</param>
    /// <returns>The current <see cref="SlotTypeValueBuilder"/>.</returns>
    public SlotTypeValueBuilder WithSynonym(string synonym)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(synonym);
        _synonyms.Add(synonym);
        return this;
    }

    /// <summary>
    /// Adds multiple synonyms for the value.
    /// </summary>
    /// <param name="synonyms">The synonyms to add.</param>
    /// <returns>The current <see cref="SlotTypeValueBuilder"/>.</returns>
    public SlotTypeValueBuilder WithSynonyms(params string[] synonyms)
    {
        ArgumentNullException.ThrowIfNull(synonyms);
        foreach (var synonym in synonyms)
        {
            WithSynonym(synonym);
        }
        return this;
    }

    /// <summary>
    /// Builds the slot type value instance.
    /// </summary>
    /// <returns>The constructed <see cref="SlotTypeValue"/>.</returns>
    public SlotTypeValue Build()
        => new()
        {
            Name = new SlotTypeValueName
            {
                Value = _value,
                Synonyms = _synonyms.Count == 0
                    ? null
                    : _synonyms.ToImmutableArray()
            }
        };
}