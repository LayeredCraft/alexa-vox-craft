using System.Text.Json;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Tests.Request;

/// <summary>
/// Request-side coverage for <see cref="AskForPermissionRequest"/>, the Connections.Response
/// counterpart of the AskForPermissionDirective (Response/DirectiveTests.cs) sent to kick off the
/// permission-consent flow.
/// </summary>
public sealed class AskForPermissionRequestTests() : TestBase<AskForPermissionRequestTests>
{
    [Fact]
    public async Task AskForPermissionRequest_Deserializes()
    {
        var json = Fx("Requests/AskForPermissionRequest.json");
        var request = JsonSerializer.Deserialize<AlexaVoxCraft.Model.Request.Type.Request>(json, AlexaJson);

        var askFor = request.Should().BeOfType<AskForPermissionRequest>().Subject;
        askFor.Name.Should().Be("AskFor");
        askFor.Status.Code.Should().Be(200);
        askFor.Payload.PermissionScope.Should().Be("alexa::alerts:reminders:skill:readwrite");
        askFor.Payload.Status.Should().Be(PermissionStatus.Denied);

        await TestHelper.VerifyRequestObject(askFor);
    }
}
