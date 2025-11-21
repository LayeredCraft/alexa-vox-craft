using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Smapi.Builders.InteractionModel;

namespace AlexaVoxCraft.Smapi.Tests.Builders.InteractionModel;

public class InteractionModelBuilderTests
{
    [Fact]
    public async Task ToJson_CreatesCorrectJson()
    {
        var json = InteractionModelBuilder.Create()
            .WithInvocationName("jedi duel")
            .WithVersion("1")
            .WithDescription("Version 1 of the Jedi Duel interaction model")
            .AddIntent("StartDuelIntent", intent =>
                intent.WithSamples("begin the duel", "I'm ready to fight", "start my battle", "let's duel",
                    "engage the opponent", "begin battle", "fight now"))
            .AddIntent("ChooseActionIntent", intent =>
                intent.WithSlot("duelAction", "DuelAction")
                    .WithSamples("I will {duelAction}", "{duelAction}", "use {duelAction}", "choose {duelAction}",
                        "I choose to {duelAction}"))
            .AddIntent("ForfeitDuelIntent", intent =>
                intent.WithSamples("give up",
                    "surrender",
                    "I surrender",
                    "forfeit",
                    "forfeit the duel",
                    "I give up",
                    "end the duel",
                    "quit the duel",
                    "stop the duel",
                    "I quit",
                    "I stop",
                    "I forfeit"))
            .AddIntent("DuelStatusIntent", intent =>
                intent.WithSamples("what's my status",
                    "what's my status",
                    "check my health",
                    "how am I doing",
                    "status report",
                    "show stats",
                    "check status",
                    "what is my status",
                    "how much health do I have",
                    "what's my health",
                    "how are we doing",
                    "show my status"))
            .AddIntent(BuiltInIntent.Cancel)
            .AddIntent(BuiltInIntent.Help)
            .AddIntent(BuiltInIntent.Stop)
            .AddIntent(BuiltInIntent.Fallback)
            .AddIntent(BuiltInIntent.NavigateHome)
            .AddIntent(BuiltInIntent.Yes)
            .AddSlotType("DuelAction", type =>
                type.WithValue("flee", v => v.WithSynonyms("run", "escape", "run away"))
                    .WithValue("use force",
                        v => v.WithSynonyms("use the force", "special power", "force attack", "force power"))
                    .WithValue("defend")
                    .WithValue("attack")
                    .WithValue("force heal", v => v.WithSynonym("heal"))
                    .WithValue("focused strike", v => v.WithSynonym("focused attack"))
                    .WithValue("power attack",
                        v => v.WithSynonyms("heavy attack", "strong attack", "devastating blow", "crushing strike")))
            .ToJson();
        await Verify(json).DisableDiff();
    }
}