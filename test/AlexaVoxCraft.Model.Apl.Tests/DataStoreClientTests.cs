using System.Net;
using AlexaVoxCraft.Model.Apl.DataStore;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Tests;

public class DataStoreClientTests
{
    private readonly ITestOutputHelper _output;

    public DataStoreClientTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task AccessTokenAsksForTheCorrectToken()
    {
        var client = new AccessTokenClient(new HttpClient(new ActionHandler(
            async hr =>
            {
                Assert.Equal("https://api.amazon.com/auth/O2/token", hr.RequestUri!.ToString());

                var content = await hr.Content!.ReadAsStringAsync();
                Assert.Equal("client_id=x&client_secret=y&grant_type=client_credentials&scope=alexa%3A%3Adatastore",
                    content);
            },
            new
            {
                access_token = "xxx",
                expires_in = 3600,
                scope = "alexa::datastore",
                token_type = "bearer"
            }
        )));

        var result = await client.Send("x", "y");
        Assert.Equal("xxx", result.Token);
        Assert.Equal("alexa::datastore", result.Scope);
    }

    [Fact]
    public async Task QueryResultSendsCorrectly()
    {
        var client = new DataStoreClient(new HttpClient(new ActionHandler( hr =>
        {
            Assert.Equal(HttpMethod.Get, hr.Method);
            Assert.Equal("https://example.com/v1/datastore/queue/x?maxResults=5&nextToken=zzz", hr.RequestUri.ToString());
            Assert.Equal("Bearer", hr.Headers.Authorization.Scheme);
            Assert.Equal("xxx", hr.Headers.Authorization.Parameter);

        }, Utility.ExampleFileContent<QueuedResultResponse>("DataStore_QueryResult.json"))), "https://example.com", "xxx");

        var result = await client.QueuedResultQuery("x",5,"zzz");
        Assert.Equal(2,result.Items.Length);
        Assert.Equal(227,result.PaginationContext.TotalCount);
    }

    [Fact]
    public async Task CancelMethodSendsCorrectly()
    {
        var client = new DataStoreClient(new HttpClient(new ActionHandler( hr =>
        {
            Assert.Equal(HttpMethod.Post, hr.Method);
            Assert.Equal("https://example.com/v1/datastore/queue/x/cancel", hr.RequestUri.ToString());
            Assert.Equal("application/json",hr.Content.Headers.ContentType.MediaType);
            Assert.Equal("Bearer", hr.Headers.Authorization.Scheme);
            Assert.Equal("xxx", hr.Headers.Authorization.Parameter);

        }, HttpStatusCode.NoContent)),"https://example.com","xxx");

        var result = await client.Cancel("x");
        Assert.True(result);
    }

    [Fact]
    public async Task CommandsMethodSendsCorrectly()
    {
        var client = new DataStoreClient(new HttpClient(new ActionHandler(async hr =>
        {
            Assert.Equal(HttpMethod.Post, hr.Method);
            Assert.Equal("https://example.com/v1/datastore/commands", hr.RequestUri.ToString());
            Assert.Equal("application/json", hr.Content.Headers.ContentType.MediaType);
            Assert.Equal("Bearer", hr.Headers.Authorization.Scheme);
            Assert.Equal("xxx", hr.Headers.Authorization.Parameter);

            var raw = await hr.Content.ReadAsStringAsync();
            var content = System.Text.Json.JsonSerializer.Deserialize<CommandsRequest>(raw, AlexaJsonOptions.DefaultOptions);
            Assert.True(Utility.CompareJson(content, "DataStore_CommandsRequest.json", _output));

        }, Utility.ExampleFileContent<CommandsResponse>("DataStore_CommandsResponse.json"))), "https://example.com", "xxx");

        var req = Utility.ExampleFileContent<CommandsRequest>("DataStore_CommandsRequest.json");
        var result = await client.Commands(req);
        Assert.True(Utility.CompareJson(result, "DataStore_CommandsResponse.json", _output));
    }
}