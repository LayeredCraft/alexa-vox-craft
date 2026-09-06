using System.Net;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using Compono.Http;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Coverage for <see cref="ProgressiveResponse"/>: the directive/request envelope it sends is
/// response-side (serialize-only, per the Constraints rule), while its HTTP-sending behavior is
/// functional and tested directly against a <see cref="TestHttpHandler"/>.
/// </summary>
public sealed class ProgressiveResponseTests() : TestBase<ProgressiveResponseTests>
{
    [Fact]
    public async Task VoicePlayerSpeakDirective_Serializes()
    {
        var directive = new VoicePlayerSpeakDirective("This text is spoken while your skill processes the full response.");

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "VoicePlayerSpeakDirective");
    }

    [Fact]
    public async Task ProgressiveResponseRequest_Serializes()
    {
        var header = new ProgressiveResponseHeader("amzn1.echo-api.request.xxxxxxx");
        var directive = new VoicePlayerSpeakDirective("This text is spoken while your skill processes the full response.");
        var request = new ProgressiveResponseRequest(header, directive);

        await TestHelper.VerifySerializedObject(request, AlexaJson, "ProgressiveResponseRequest");
    }

    [Fact]
    public async Task Send_WithNoDetail_ReturnsNull()
    {
        var response = new ProgressiveResponse();

        var result = await response.Send(null);

        result.Should().BeNull();
    }

    [Fact]
    public void CanSend_WithNoDetail_ReturnsFalse()
    {
        var response = new ProgressiveResponse();

        response.CanSend().Should().BeFalse();
    }

    [Fact]
    public async Task Send_WithNoHeader_ReturnsNull()
    {
        var response = new ProgressiveResponse { Client = new HttpClient() };

        var result = await response.Send(new VoicePlayerSpeakDirective("test"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Send_WithNoClient_ReturnsNull()
    {
        var response = new ProgressiveResponse { Header = new ProgressiveResponseHeader("test") };

        var result = await response.Send(new VoicePlayerSpeakDirective("test"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Send_WithNullDirective_ReturnsNull()
    {
        var response = new ProgressiveResponse { Header = new ProgressiveResponseHeader("test"), Client = new HttpClient() };

        var result = await response.Send(null);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SendSpeech_CallsBaseAddress()
    {
        using var handler = new TestHttpHandler();
        handler.OnPost("/v1/directives").Respond(HttpStatusCode.NoContent);
        var response = new ProgressiveResponse("xxx", "authToken", "http://localhost", handler.CreateClient());

        var result = await response.SendSpeech("hello");

        handler.Requests.Should().ContainSingle().Which.RequestUri!.Host.Should().Be("localhost");
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SendSpeech_CallsDirectiveApi()
    {
        using var handler = new TestHttpHandler();
        handler.OnPost("/v1/directives").Respond(HttpStatusCode.NoContent);
        var response = new ProgressiveResponse("xxx", "authToken", "http://localhost", handler.CreateClient());

        await response.SendSpeech("hello");

        handler.Requests.Should().ContainSingle().Which.RequestUri!.AbsolutePath.Should().Be("/v1/directives");
    }

    [Fact]
    public async Task SendSpeech_SetsBearerAuthorizationHeader()
    {
        using var handler = new TestHttpHandler();
        handler.OnPost("/v1/directives").Respond(HttpStatusCode.NoContent);
        var response = new ProgressiveResponse("xxx", "authToken", "http://localhost", handler.CreateClient());

        await response.SendSpeech("hello");

        var request = handler.Requests.Should().ContainSingle().Subject;
        request.Headers.Authorization!.Scheme.Should().Be("Bearer");
        request.Headers.Authorization.Parameter.Should().Be("authToken");
    }
}
