using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Compono.XunitV3.Aot;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Scenario 5 (original console app): SkillMediator.Send through the generator-produced keyed
// dispatch path.
public sealed class MediatorTests
{
    [Theory]
    [Compose<MediatorProfile, MediatorSkillId>("amzn1.ask.skill.validation")]
    public async Task SkillMediator_Send_DispatchesViaGeneratedHandler(ISkillMediator mediator)
    {
        var skillRequest = new SkillRequest
        {
            Request = new LaunchRequest { Type = "LaunchRequest" },
            Context = new Context
            {
                System = new AlexaSystem { Application = new Application { ApplicationId = "amzn1.ask.skill.validation" } }
            }
        };
        AmbientRequest.Current = skillRequest;

        var response = await mediator.Send(skillRequest, CancellationToken.None);

        var ssml = Assert.IsType<SsmlOutputSpeech>(response.Response?.OutputSpeech);
        Assert.Contains("hello from the native AOT test project", ssml.Ssml);
    }
}
