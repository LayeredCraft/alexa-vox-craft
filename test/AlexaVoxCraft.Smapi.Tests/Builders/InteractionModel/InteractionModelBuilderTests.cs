using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Smapi.Builders.InteractionModel;
using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Tests.Builders.InteractionModel;

public class InteractionModelBuilderTests
{
    [Fact]
    public async Task ToJson_CreatesCorrectJson()
    {
        var json = InteractionModelBuilder.Create()
            .WithInvocationName("sample skill")
            .WithVersion("1")
            .WithDescription("Version 1 of the sample interaction model")
            .AddIntent("StartGameIntent", intent =>
                intent.WithSamples("start game", "begin", "let's play", "start", "new game"))
            .AddIntent("ChooseActionIntent", intent =>
                intent.WithSlot("action", "GameAction")
                    .WithSamples("I will {action}", "{action}", "use {action}", "choose {action}"))
            .AddIntent("QuitIntent", intent =>
                intent.WithSamples("quit", "stop", "exit", "end game"))
            .AddIntent("StatusIntent", intent =>
                intent.WithSamples("what's my status", "status", "how am I doing", "show stats"))
            .AddIntent(BuiltInIntent.Cancel)
            .AddIntent(BuiltInIntent.Help)
            .AddIntent(BuiltInIntent.Stop)
            .AddIntent(BuiltInIntent.Fallback)
            .AddIntent(BuiltInIntent.NavigateHome)
            .AddIntent(BuiltInIntent.Yes)
            .AddSlotType("GameAction", type =>
                type.WithValue("move", v => v.WithSynonyms("go", "walk"))
                    .WithValue("jump", v => v.WithSynonyms("leap", "hop"))
                    .WithValue("attack", v => v.WithSynonyms("strike", "hit")))
            .ToJson();
        await Verify(json).DisableDiff();
    }

    [Fact]
    public void BuildLocalized_WithLocale_ReturnsLocalizedModel()
    {
        var result = InteractionModelBuilder.Create()
            .WithLocale("en-US")
            .WithInvocationName("my skill")
            .WithVersion("1")
            .WithDescription("Test")
            .AddIntent(BuiltInIntent.Cancel)
            .BuildLocalized();

        result.Should().BeOfType<LocalizedInteractionModel>();
        result.Locale.Should().Be("en-US");
        result.Definition.Should().NotBeNull();
        result.Definition.InteractionModel.LanguageModel.InvocationName.Should().Be("my skill");
    }

    [Fact]
    public void BuildLocalized_WithoutLocale_ThrowsInvalidOperationException()
    {
        var builder = InteractionModelBuilder.Create()
            .WithInvocationName("my skill")
            .WithVersion("1")
            .WithDescription("Test")
            .AddIntent(BuiltInIntent.Cancel);

        var act = () => builder.BuildLocalized();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("A locale must be specified using WithLocale.");
    }

    [Fact]
    public async Task ToJson_WithNameFreeInteraction_CreatesCorrectJson()
    {
        var json = InteractionModelBuilder.Create()
            .WithInvocationName("coffee shop")
            .WithVersion("1")
            .WithDescription("Coffee shop with name-free interactions")
            .AddIntent("OrderIntent", intent => intent
                .WithSamples("order {drink}", "buy {drink}")
                .WithSlot("drink", "DrinkType"))
            .AddSlotType("DrinkType", type => type
                .WithValue("coffee")
                .WithValue("tea"))
            .WithNameFreeInteraction(nfi => nfi
                .WithLaunchIngressPoint(launch => launch
                    .WithUtterances("what's available", "show menu"))
                .WithIntentIngressPoint("OrderIntent", intent => intent
                    .WithUtterances("order coffee", "get tea")))
            .ToJson();

        await Verify(json).DisableDiff();
    }
}