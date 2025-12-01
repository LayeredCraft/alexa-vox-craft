using System.Collections.Immutable;
using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Builders.InteractionModel;

public sealed class NameFreeInteractionBuilder
{
    private readonly List<NameFreeIngressPointBuilder> _ingressPoints = [];

    public NameFreeInteractionBuilder WithLaunchIngressPoint(Action<NameFreeIngressPointBuilder>? configure = null)
    {
        var builder = NameFreeIngressPointBuilder.Launch();
        configure?.Invoke(builder);
        _ingressPoints.Add(builder);
        return this;
    }

    public NameFreeInteractionBuilder WithIntentIngressPoint(string intentName, Action<NameFreeIngressPointBuilder>? configure = null)
    {
        var builder = NameFreeIngressPointBuilder.Intent(intentName);
        configure?.Invoke(builder);
        _ingressPoints.Add(builder);
        return this;
    }

    public NameFreeInteractionBuilder WithIngressPoint(Action<NameFreeIngressPointBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = NameFreeIngressPointBuilder.Launch();
        configure(builder);
        _ingressPoints.Add(builder);
        return this;
    }

    public NameFreeInteraction Build()
    {
        var ingressPoints = _ingressPoints
            .Select(static ip => ip.Build())
            .ToImmutableArray();

        return new NameFreeInteraction
        {
            IngressPoints = ingressPoints
        };
    }
}