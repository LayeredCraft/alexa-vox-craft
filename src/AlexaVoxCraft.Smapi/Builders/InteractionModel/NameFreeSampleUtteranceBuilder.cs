using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Builders.InteractionModel;

public sealed class NameFreeSampleUtteranceBuilder
{
    private readonly string _value;
    private string _format = "RAW_TEXT";

    public NameFreeSampleUtteranceBuilder(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        _value = value;
    }

    public NameFreeSampleUtteranceBuilder WithFormat(string format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        _format = format;
        return this;
    }

    public NameFreeSampleUtterance Build()
    {
        return new NameFreeSampleUtterance
        {
            Format = _format,
            Value = _value
        };
    }
}