using AlexaVoxCraft.Smapi.Builders.InteractionModel;

namespace AlexaVoxCraft.Smapi.Tests.Builders.InteractionModel;

public sealed class NameFreeInteractionBuilderTests
{
    [Fact]
    public async Task Build_WithLaunchIngressPoint_CreatesCorrectModel()
    {
        var nfi = new NameFreeInteractionBuilder()
            .WithLaunchIngressPoint(launch => launch
                .WithUtterances("what's new", "latest updates", "recent changes"))
            .Build();

        await Verify(nfi);
    }

    [Fact]
    public async Task Build_WithIntentIngressPoint_CreatesCorrectModel()
    {
        var nfi = new NameFreeInteractionBuilder()
            .WithIntentIngressPoint("OrderIntent", intent => intent
                .WithUtterances("order coffee", "buy a latte", "get me tea"))
            .Build();

        await Verify(nfi);
    }

    [Fact]
    public async Task Build_WithMultipleIngressPoints_CreatesCorrectModel()
    {
        var nfi = new NameFreeInteractionBuilder()
            .WithLaunchIngressPoint(launch => launch
                .WithUtterances("what's new", "latest"))
            .WithIntentIngressPoint("OrderIntent", intent => intent
                .WithUtterances("order coffee", "buy tea"))
            .WithIntentIngressPoint("StatusIntent", intent => intent
                .WithUtterance("check status"))
            .Build();

        await Verify(nfi);
    }

    [Fact]
    public async Task Build_WithConfiguredUtteranceFormat_CreatesCorrectModel()
    {
        var nfi = new NameFreeInteractionBuilder()
            .WithLaunchIngressPoint(launch => launch
                .WithUtterance("custom format", utterance => utterance
                    .WithFormat("CUSTOM_FORMAT")))
            .Build();

        await Verify(nfi);
    }

    [Fact]
    public async Task Build_WithMixedUtteranceStyles_CreatesCorrectModel()
    {
        var nfi = new NameFreeInteractionBuilder()
            .WithIntentIngressPoint("MixedIntent", intent => intent
                .WithUtterance("simple utterance")
                .WithUtterances("another one", "and another")
                .WithUtterance("configured", u => u.WithFormat("SPECIAL")))
            .Build();

        await Verify(nfi);
    }
}