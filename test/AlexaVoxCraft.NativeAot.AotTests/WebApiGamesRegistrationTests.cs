using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.WebApiGames;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Issue #213: WebApiGamesSupport.Add() runs once in AssemblyModuleInitializer alongside APLSupport.Add();
// a repeated call must stay idempotent and leave the directive/request registrations intact.
public sealed class WebApiGamesRegistrationTests
{
    [Fact]
    public void Add_IsIdempotent_WhenRepeated()
    {
        for (var i = 0; i < 25; i++)
        {
            WebApiGamesSupport.Add();
        }

        var directive = JsonSerializer.Deserialize(
            """{"type":"Alexa.Presentation.HTML.HandleMessage","message":{"Action":"x"}}""",
            AlexaJsonTypeInfo.For<IDirective>());
        Assert.IsType<HandleMessageDirective>(directive);

        var envelope = JsonSerializer.Deserialize(
            """{"version":"1.0","request":{"type":"Alexa.Presentation.HTML.Message","message":{}}}""",
            AlexaJsonTypeInfo.For<SkillRequest>());
        Assert.NotNull(envelope);
        Assert.IsType<MessageRequest>(envelope.Request);
    }
}
