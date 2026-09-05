using System.Net;
using AlexaVoxCraft.Model.Apl.DataStore;
using Compono.Http;

namespace AlexaVoxCraft.Model.Apl.Tests.DataStore;

/// <summary>
/// Coverage for <see cref="AccessTokenClient"/> and <see cref="DataStoreClient"/>: unlike a directive
/// or component, these are HTTP clients that genuinely send AND receive JSON over real HTTP calls, so
/// (like <c>ProgressiveResponseTests</c> in Model.Tests) they're tested directly against a
/// <see cref="TestHttpHandler"/> rather than treated as one-directional per the envelope-role rule.
/// </summary>
public sealed class DataStoreClientTests
{
    [Fact]
    public async Task AccessTokenClient_Send_PostsCorrectRequestAndParsesResponse()
    {
        using var handler = new TestHttpHandler();
        handler.OnPost("/auth/O2/token").RespondJson(new
        {
            access_token = "xxx",
            expires_in = 3600,
            scope = "alexa::datastore",
            token_type = "bearer"
        });
        var client = new AccessTokenClient(handler.CreateClient(new Uri("https://api.amazon.com")));

        var result = await client.Send("x", "y");

        result.Token.Should().Be("xxx");
        result.Scope.Should().Be("alexa::datastore");

        var request = handler.Requests.Should().ContainSingle().Subject;
        request.RequestUri.Should().Be(new Uri("https://api.amazon.com/auth/O2/token"));
        var content = await request.Content!.ReadAsStringAsync(TestContext.Current.CancellationToken);
        content.Should().Be("client_id=x&client_secret=y&grant_type=client_credentials&scope=alexa%3A%3Adatastore");
    }

    [Fact]
    public async Task DataStoreClient_QueuedResultQuery_SendsCorrectRequestAndParsesResponse()
    {
        using var handler = new TestHttpHandler();
        handler.OnGet("/v1/datastore/queue/x?maxResults=5&nextToken=zzz").RespondJson(new QueuedResultResponse
        {
            Items = [new CommandResult { DeviceId = "device-1", Type = CommandResultType.Success }],
            PaginationContext = new PaginationContext { TotalCount = 227 }
        });
        var client = new DataStoreClient(handler.CreateClient(), new Uri("https://example.com").ToString(), "xxx");

        var result = await client.QueuedResultQuery("x", 5, "zzz");

        result.Items.Should().HaveCount(1);
        result.PaginationContext.TotalCount.Should().Be(227);

        var request = handler.Requests.Should().ContainSingle().Subject;
        request.Method.Should().Be(HttpMethod.Get);
        request.RequestUri.Should().Be(new Uri("https://example.com/v1/datastore/queue/x?maxResults=5&nextToken=zzz"));
        request.Headers.Authorization!.Scheme.Should().Be("Bearer");
        request.Headers.Authorization.Parameter.Should().Be("xxx");
    }

    [Fact]
    public async Task DataStoreClient_Cancel_SendsCorrectRequest()
    {
        using var handler = new TestHttpHandler();
        handler.OnPost("/v1/datastore/queue/x/cancel").Respond(HttpStatusCode.NoContent);
        var client = new DataStoreClient(handler.CreateClient(), "https://example.com", "xxx");

        var result = await client.Cancel("x");

        result.Should().BeTrue();

        var request = handler.Requests.Should().ContainSingle().Subject;
        request.Method.Should().Be(HttpMethod.Post);
        request.RequestUri.Should().Be(new Uri("https://example.com/v1/datastore/queue/x/cancel"));
        request.Content!.Headers.ContentType!.MediaType.Should().Be("application/json");
        request.Headers.Authorization!.Scheme.Should().Be("Bearer");
        request.Headers.Authorization.Parameter.Should().Be("xxx");
    }

    [Fact]
    public async Task DataStoreClient_Commands_SendsCorrectRequestAndParsesResponse()
    {
        using var handler = new TestHttpHandler();
        handler.OnPost("/v1/datastore/commands").RespondJson(new CommandsResponse
        {
            QueuedResultId = "queued-result-id",
            Results = [new CommandResult { DeviceId = "device-1", Type = CommandResultType.Success }]
        });
        var client = new DataStoreClient(handler.CreateClient(), "https://example.com", "xxx");
        var req = new CommandsRequest { Commands = [new Clear()] };

        var result = await client.Commands(req);

        result.QueuedResultId.Should().Be("queued-result-id");
        result.Results.Should().ContainSingle();

        var request = handler.Requests.Should().ContainSingle().Subject;
        request.Method.Should().Be(HttpMethod.Post);
        request.RequestUri.Should().Be(new Uri("https://example.com/v1/datastore/commands"));
        request.Content!.Headers.ContentType!.MediaType.Should().Be("application/json");
        request.Headers.Authorization!.Scheme.Should().Be("Bearer");
        request.Headers.Authorization.Parameter.Should().Be("xxx");
    }
}
