using System.Collections.Immutable;
using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Builders.InteractionModel;

/// <summary>
/// Fluent builder for constructing custom slot types.
/// </summary>
public sealed class SlotTypeBuilder
{
    private readonly string _name;
    private readonly List<SlotTypeValueBuilder> _values = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SlotTypeBuilder"/> class.
    /// </summary>
    /// <param name="name">The slot type name.</param>
    public SlotTypeBuilder(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _name = name;
    }

    /// <summary>
    /// Adds a value to the slot type.
    /// </summary>
    /// <param name="value">The primary value.</param>
    /// <param name="configure">Optional configuration action.</param>
    /// <returns>The current <see cref="SlotTypeBuilder"/>.</returns>
    public SlotTypeBuilder WithValue(
        string value,
        Action<SlotTypeValueBuilder>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var builder = new SlotTypeValueBuilder(value);
        configure?.Invoke(builder);
        _values.Add(builder);
        return this;
    }

    /// <summary>
    /// Builds the slot type instance.
    /// </summary>
    /// <returns>The constructed <see cref="SlotType"/>.</returns>
    public SlotType Build()
    {
        var values = _values
            .Select(static v => v.Build())
            .ToImmutableArray();

        return new SlotType
        {
            Name = _name,
            Values = values
        };
    }
}