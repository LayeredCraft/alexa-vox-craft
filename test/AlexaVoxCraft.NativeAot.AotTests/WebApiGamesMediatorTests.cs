using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.WebApiGames;
using Compono.XunitV3.Aot;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Issue #213: a Web API for Games MessageRequest deserialized through AlexaJsonOptions reaches a typed
// IRequestHandler<MessageRequest> through the real generator-produced keyed dispatch path, and the
// response's HandleMessageDirective serializes back out under reflection-disabled Native AOT.
public sealed class WebApiGamesMediatorTests
{
    private const string SkillId = "amzn1.ask.skill.webapigames";

    [Theory]
    [Compose<MediatorProfile, MediatorSkillId>(SkillId)]
    public async Task SkillMediator_Send_MessageRequest_DispatchesGeneratedHandler_AndSerializesDirective(
        MediatorTestScope mediatorScope)
    {
        using var _ = mediatorScope;

        var json =
            "{\"version\":\"1.0\"," +
            "\"session\":{\"new\":true,\"sessionId\":\"s1\",\"application\":{\"applicationId\":\"" + SkillId + "\"},\"user\":{\"userId\":\"u1\"}}," +
            "\"context\":{\"System\":{\"application\":{\"applicationId\":\"" + SkillId + "\"}," +
            "\"user\":{\"userId\":\"u1\"},\"device\":{\"deviceId\":\"d1\",\"supportedInterfaces\":{\"Alexa.Presentation.HTML\":{}}}}}," +
            "\"request\":{\"type\":\"Alexa.Presentation.HTML.Message\",\"requestId\":\"r1\"," +
            "\"timestamp\":\"2024-05-01T12:00:00Z\",\"locale\":\"en-US\"," +
            "\"message\":{\"action\":\"move\",\"level\":2}}}";

        var request = JsonSerializer.Deserialize(json, AlexaJsonTypeInfo.For<SkillRequest>());
        Assert.NotNull(request);
        Assert.IsType<MessageRequest>(request.Request);

        AmbientRequest.Current = request;

        try
        {
            var response = await mediatorScope.Mediator.Send(request, CancellationToken.None);

            var serialized = JsonSerializer.Serialize(response, AlexaJsonTypeInfo.For<SkillResponse>());
            Assert.Contains("Alexa.Presentation.HTML.HandleMessage", serialized);
            Assert.Contains("\"move\"", serialized);
        }
        finally
        {
            AmbientRequest.Current = null;
        }
    }
}
