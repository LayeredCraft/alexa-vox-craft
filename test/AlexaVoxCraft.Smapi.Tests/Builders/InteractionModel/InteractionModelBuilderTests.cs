using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Smapi.Builders.InteractionModel;

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
}