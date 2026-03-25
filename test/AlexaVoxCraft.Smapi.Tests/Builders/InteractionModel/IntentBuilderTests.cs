using AlexaVoxCraft.Smapi.Builders.InteractionModel;

namespace AlexaVoxCraft.Smapi.Tests.Builders.InteractionModel;

public sealed class IntentBuilderTests
{
    [Fact]
    public async Task Build_WithNoSamplesOrSlots_ReturnsIntentWithEmptyCollections()
    {
        var intent = new IntentBuilder("OrderIntent")
            .Build();

        await Verify(intent).DisableDiff();
    }

    [Fact]
    public async Task Build_WithSingleSample_IncludesSampleInResult()
    {
        var intent = new IntentBuilder("OrderIntent")
            .WithSample("order {drink}")
            .Build();

        await Verify(intent).DisableDiff();
    }

    [Fact]
    public async Task Build_WithMultipleSamplesViaWithSample_IncludesAllSamples()
    {
        var intent = new IntentBuilder("OrderIntent")
            .WithSample("order {drink}")
            .WithSample("get {drink}")
            .WithSample("I'd like {drink}")
            .Build();

        await Verify(intent).DisableDiff();
    }

    [Fact]
    public async Task Build_WithMultipleSamplesViaWithSamples_IncludesAllSamples()
    {
        var intent = new IntentBuilder("OrderIntent")
            .WithSamples("order {drink}", "get {drink}", "I'd like {drink}")
            .Build();

        await Verify(intent).DisableDiff();
    }

    [Fact]
    public async Task Build_WithSingleSlot_IncludesSlotInResult()
    {
        var intent = new IntentBuilder("OrderIntent")
            .WithSample("order {drink}")
            .WithSlot("drink", "DrinkType")
            .Build();

        await Verify(intent).DisableDiff();
    }

    [Fact]
    public async Task Build_WithMultipleSlots_IncludesAllSlots()
    {
        var intent = new IntentBuilder("OrderIntent")
            .WithSample("order {drink} in {size}")
            .WithSlot("drink", "DrinkType")
            .WithSlot("size", "SizeType")
            .Build();

        await Verify(intent).DisableDiff();
    }

    [Fact]
    public async Task Build_WithConfiguredSlot_IncludesSlotConfiguration()
    {
        var intent = new IntentBuilder("OrderIntent")
            .WithSample("order {drink}")
            .WithSlot("drink", "DrinkType", slot => slot.Required().WithSample("{drink}"))
            .Build();

        await Verify(intent).DisableDiff();
    }

    [Fact]
    public void WithSlot_WithDuplicateSlotName_ThrowsInvalidOperationException()
    {
        var builder = new IntentBuilder("OrderIntent")
            .WithSlot("drink", "DrinkType");

        var act = () => builder.WithSlot("drink", "AnotherType");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Slot 'drink' has already been added to intent 'OrderIntent'.");
    }

    [Fact]
    public void WithSlot_WhenConfigureThrows_SlotNameIsNotRetained()
    {
        var builder = new IntentBuilder("OrderIntent");

        var act = () => builder.WithSlot("drink", "DrinkType", _ => throw new InvalidOperationException("configure failed"));

        act.Should().Throw<InvalidOperationException>().WithMessage("configure failed");

        var intent = builder.WithSlot("drink", "DrinkType").Build();
        intent.Slots.Should().HaveCount(1);
    }
}