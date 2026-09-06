using System.Text.Json;
using AlexaVoxCraft.Model.ConnectionTasks.Inputs;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response.Converters;
using AlexaVoxCraft.Model.Tests.Infrastructure;

namespace AlexaVoxCraft.Model.Tests.Request;

/// <summary>
/// Request-side coverage for <see cref="SessionResumedRequest"/> and <see cref="LaunchRequest"/>'s
/// <c>task</c> field, including the custom-task extensibility hook. Generic print/task connections
/// are unused by the trivia skill, so all fixtures are synthetic (built fresh, not reused from the
/// Legacy project).
/// </summary>
public sealed class ConnectionTaskRequestTests() : TestBase<ConnectionTaskRequestTests>
{
    [Fact]
    public async Task SessionResumedRequest_Deserializes()
    {
        var json = Fx("Requests/SessionResumedRequest.json");
        var envelope = JsonSerializer.Deserialize<AlexaVoxCraft.Model.Request.Type.Request>(json, AlexaJson);

        var request = envelope.Should().BeOfType<SessionResumedRequest>().Subject;
        request.Cause.Token.Should().Be("correlation-token-example");
        request.Cause.Status.Code.Should().Be(200);
        request.Cause.Status.Message.Should().Be("OK");

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task LaunchRequest_WithTask_Deserializes()
    {
        var json = Fx("Requests/LaunchRequestWithTask.json");
        var envelope = JsonSerializer.Deserialize<LaunchRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        envelope!.Task.Should().NotBeNull();
        envelope.Task.Name.Should().Be("AMAZON.PrintPDF");
        envelope.Task.Version.Should().Be("1");
        envelope.Task.Input.Should().BeOfType<PrintPdfV1>();

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task LaunchRequest_WithCustomTask_Deserializes()
    {
        ConnectionTaskConverter.AddToConnectionTaskResolvers(new ExampleConnectionTaskResolver());

        var json = Fx("Requests/LaunchRequestWithCustomTask.json");
        var envelope = JsonSerializer.Deserialize<LaunchRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        envelope!.Task.Name.Should().Be("Custom.ExampleTask");
        var task = envelope.Task.Input.Should().BeOfType<ExampleConnectionTask>().Subject;
        task.RandomParameter.Should().Be("parameterValue");
    }

    [Fact]
    public void PinConfirmationResult_ResolvesFromSessionResumedRequest()
    {
        var json = Fx("Requests/SessionResumedRequestWithPinConfirmationResult.json");
        var envelope = JsonSerializer.Deserialize<SessionResumedRequest>(json, AlexaJson);

        var result = PinConfirmationResolver.ResultFromSessionResumed(envelope!);

        result.Should().NotBeNull();
        result!.Status.Should().Be(PinConfirmationStatus.NotAchieved);
        result.Reason.Should().Be(PinConfirmationReason.VerificationMethodNotSetup);
    }
}
