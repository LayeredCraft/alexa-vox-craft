using System.Collections.Immutable;
using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Builders.InteractionModel;

public sealed class NameFreeIngressPointBuilder
{
    private readonly string _type;
    private readonly string? _name;
    private readonly List<NameFreeSampleUtteranceBuilder> _sampleUtterances = [];

    private NameFreeIngressPointBuilder(string type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        _type = type;
        _name = null;
    }

    private NameFreeIngressPointBuilder(string type, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _type = type;
        _name = name;
    }

    public static NameFreeIngressPointBuilder Launch()
        => new("LAUNCH");

    public static NameFreeIngressPointBuilder Intent(string intentName)
        => new("INTENT", intentName);

    public NameFreeIngressPointBuilder WithUtterance(string utterance, Action<NameFreeSampleUtteranceBuilder>? configure = null)
    {
        var builder = new NameFreeSampleUtteranceBuilder(utterance);
        configure?.Invoke(builder);
        _sampleUtterances.Add(builder);
        return this;
    }

    public NameFreeIngressPointBuilder WithUtterances(params string[] utterances)
    {
        ArgumentNullException.ThrowIfNull(utterances);
        foreach (var utterance in utterances)
            WithUtterance(utterance);
        return this;
    }

    public NameFreeIngressPoint Build()
    {
        var utterances = _sampleUtterances
            .Select(static u => u.Build())
            .ToImmutableArray();

        return new NameFreeIngressPoint
        {
            Type = _type,
            Name = _name,
            SampleUtterances = utterances
        };
    }
}