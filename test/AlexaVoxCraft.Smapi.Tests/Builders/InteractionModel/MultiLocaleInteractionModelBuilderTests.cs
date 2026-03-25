using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Smapi.Builders.InteractionModel;

namespace AlexaVoxCraft.Smapi.Tests.Builders.InteractionModel;

public sealed class MultiLocaleInteractionModelBuilderTests
{
    [Fact]
    public async Task BuildAll_WithSingleDefaultLocale_ReturnsOneModel()
    {
        var models = MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Single locale skill")
            .AddIntent("HelloIntent", i => i.WithSamples("say hello", "hello"))
            .AddIntent(BuiltInIntent.Cancel)
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill")
                .WithIntentSamples("HelloIntent", "say hello", "hello"))
            .BuildAll();

        await Verify(models).DisableDiff();
    }

    [Fact]
    public async Task BuildAll_WithMultipleLocales_ReturnsAllLocales()
    {
        var models = MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Multi-locale skill")
            .AddIntent("OrderIntent", i => i.WithSlot("drink", "DrinkType"))
            .AddIntent(BuiltInIntent.Cancel)
            .AddIntent(BuiltInIntent.Stop)
            .AddSlotType("DrinkType")
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill")
                .WithIntentSamples("OrderIntent", "order {drink}", "get {drink}")
                .WithSlotValues("DrinkType", v => v.WithValue("coffee").WithValue("tea")))
            .ForLocale("en-CA")
            .ForLocale("en-GB", locale => locale
                .WithIntentSamples("OrderIntent", "order {drink}", "I'd like {drink}"))
            .BuildAll();

        await Verify(models).DisableDiff();
    }

    [Fact]
    public async Task BuildAll_WithLocaleOverridingInvocationName_UsesOverrideForThatLocale()
    {
        var models = MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Invocation name override test")
            .AddIntent(BuiltInIntent.Cancel)
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill"))
            .ForLocale("en-GB", locale => locale
                .WithInvocationName("my british skill"))
            .ForLocale("en-CA")
            .BuildAll();

        await Verify(models).DisableDiff();
    }

    [Fact]
    public async Task BuildAll_WithIntentSamplesOverride_AppliesOverrideForLocale()
    {
        var models = MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Intent samples override test")
            .AddIntent("GreetIntent")
            .AddIntent(BuiltInIntent.Stop)
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill")
                .WithIntentSamples("GreetIntent", "say hello", "greet me"))
            .ForLocale("en-GB", locale => locale
                .WithIntentSamples("GreetIntent", "say hello", "good day"))
            .ForLocale("en-CA")
            .BuildAll();

        await Verify(models).DisableDiff();
    }

    [Fact]
    public async Task BuildAll_WithSlotSamplesOverride_AppliesOverrideForLocale()
    {
        var models = MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Slot samples override test")
            .AddIntent("OrderIntent", i => i.WithSlot("item", "AMAZON.Food"))
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill")
                .WithIntentSamples("OrderIntent", "order {item}")
                .WithSlotSamples("OrderIntent", "item", "coffee", "sandwich"))
            .ForLocale("en-GB", locale => locale
                .WithSlotSamples("OrderIntent", "item", "tea", "biscuit"))
            .BuildAll();

        await Verify(models).DisableDiff();
    }

    [Fact]
    public async Task BuildAll_WithSlotValuesOverride_AppliesOverrideForLocale()
    {
        var models = MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Slot values override test")
            .AddIntent("OrderIntent", i => i.WithSlot("drink", "DrinkType"))
            .AddSlotType("DrinkType")
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill")
                .WithIntentSamples("OrderIntent", "order {drink}")
                .WithSlotValues("DrinkType", v => v.WithValue("soda").WithValue("coffee")))
            .ForLocale("en-GB", locale => locale
                .WithSlotValues("DrinkType", v => v.WithValue("fizzy drink").WithValue("coffee")))
            .BuildAll();

        await Verify(models).DisableDiff();
    }

    [Fact]
    public async Task BuildAll_WithForLocaleAndNoLambda_InheritsAllFromDefault()
    {
        var models = MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Inherit all from default")
            .AddIntent("HelloIntent")
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill")
                .WithIntentSamples("HelloIntent", "say hello", "hello there"))
            .ForLocale("en-CA")
            .ForLocale("en-AU")
            .BuildAll();

        await Verify(models).DisableDiff();
    }

    [Fact]
    public async Task BuildAll_WithDuplicateForLocale_MergesIntoSameBuilder()
    {
        var models = MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Duplicate locale merge test")
            .AddIntent("FooIntent")
            .AddIntent("BarIntent")
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill")
                .WithIntentSamples("FooIntent", "do foo")
                .WithIntentSamples("BarIntent", "do bar"))
            .ForLocale("en-GB", locale => locale
                .WithIntentSamples("FooIntent", "do foo differently"))
            .ForLocale("en-GB", locale => locale
                .WithInvocationName("my british skill"))
            .BuildAll();

        await Verify(models).DisableDiff();
    }

    [Fact]
    public void BuildAll_WithNoDefaultLocale_ThrowsInvalidOperationException()
    {
        var builder = MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Missing default locale")
            .AddIntent(BuiltInIntent.Cancel);

        var act = () => builder.BuildAll();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("A default locale must be specified using WithDefaultLocale.");
    }

    [Fact]
    public void WithIntentSamples_ForUnknownIntent_ThrowsInvalidOperationException()
    {
        var act = () => MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Unknown intent test")
            .AddIntent("KnownIntent")
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill")
                .WithIntentSamples("UnknownIntent", "some sample"));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Intent 'UnknownIntent' is not defined in the interaction model schema.");
    }

    [Fact]
    public void WithSlotSamples_ForUnknownSlot_ThrowsInvalidOperationException()
    {
        var act = () => MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Unknown slot test")
            .AddIntent("OrderIntent", i => i.WithSlot("drink", "DrinkType"))
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill")
                .WithIntentSamples("OrderIntent", "order {drink}")
                .WithSlotSamples("OrderIntent", "unknownSlot", "some value"));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Slot 'unknownSlot' is not defined on intent 'OrderIntent'.");
    }

    [Fact]
    public void WithSlotValues_ForUnknownSlotType_ThrowsInvalidOperationException()
    {
        var act = () => MultiLocaleInteractionModelBuilder.Create()
            .WithVersion("1")
            .WithDescription("Unknown slot type test")
            .AddSlotType("KnownType")
            .WithDefaultLocale("en-US", locale => locale
                .WithInvocationName("my skill")
                .WithSlotValues("UnknownType", v => v.WithValue("foo")));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Slot type 'UnknownType' is not defined in the interaction model schema.");
    }
}
