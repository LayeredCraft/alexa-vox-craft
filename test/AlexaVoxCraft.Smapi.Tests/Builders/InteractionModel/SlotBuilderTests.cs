using AlexaVoxCraft.Smapi.Builders.InteractionModel;

namespace AlexaVoxCraft.Smapi.Tests.Builders.InteractionModel;

public sealed class SlotBuilderTests
{
    [Fact]
    public async Task Build_WithNoSamples_OmitsSamplesFromResult()
    {
        var slot = new SlotBuilder("productCategory", "factType")
            .Build();

        await Verify(slot).DisableDiff();
    }

    [Fact]
    public async Task Build_WithSingleSample_IncludesSampleInResult()
    {
        var slot = new SlotBuilder("productCategory", "factType")
            .WithSample("{productCategory} pack")
            .Build();

        await Verify(slot).DisableDiff();
    }

    [Fact]
    public async Task Build_WithMultipleSamplesViaWithSample_IncludesAllSamples()
    {
        var slot = new SlotBuilder("productCategory", "factType")
            .WithSample("{productCategory} pack")
            .WithSample("Tell me about a {productCategory} pack")
            .WithSample("Tell me about the {productCategory} pack")
            .Build();

        await Verify(slot).DisableDiff();
    }

    [Fact]
    public async Task Build_WithMultipleSamplesViaWithSamples_IncludesAllSamples()
    {
        var slot = new SlotBuilder("productCategory", "factType")
            .WithSamples(
                "{productCategory} pack",
                "Tell me about a {productCategory} pack",
                "Tell me about the {productCategory} pack",
                "Tell me about {productCategory} pack")
            .Build();

        await Verify(slot).DisableDiff();
    }

    [Fact]
    public async Task Build_WithSamplesAndRequired_CombinesPropertiesCorrectly()
    {
        var slot = new SlotBuilder("productCategory", "factType")
            .Required()
            .WithSamples("{productCategory} pack", "Tell me about a {productCategory} pack")
            .Build();

        await Verify(slot).DisableDiff();
    }
}