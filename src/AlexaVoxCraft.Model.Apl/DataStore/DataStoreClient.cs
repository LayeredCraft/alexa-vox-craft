using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class DataStoreClient
{
    public HttpClient Client { get; }
    public Uri BaseAddress { get; }
    private string Token { get; }

    public DataStoreClient(SkillRequest request) : this(
        request.Context.System.ApiEndpoint,
        request.Context.System.ApiAccessToken)
    {
    }

    public DataStoreClient(string endpointUrl, string accessToken):this(null, endpointUrl, accessToken)
    {
            
    }

    public DataStoreClient(HttpClient client, string endpointUrl, string accessToken)
    {
        Client = client ?? new HttpClient();
        BaseAddress = new Uri(endpointUrl);
        Token = accessToken;
    }

    public Task<QueuedResultResponse> QueuedResultQuery(string queuedResultId, int maxResults)
    {
        return QueuedResultQuery(queuedResultId, maxResults, null);
    }

    public async Task<QueuedResultResponse> QueuedResultQuery(string queuedResultId, int? maxResults = null, string nextToken = null)
    {
        var url = $"/v1/datastore/queue/{queuedResultId}";
        var query = "";
        if (maxResults != null || !string.IsNullOrWhiteSpace(nextToken))
        {
            query = "?";
            if (maxResults != null)
            {
                query += "maxResults=";
                query += maxResults.ToString();
            }

            if (!string.IsNullOrWhiteSpace(nextToken))
            {
                if (query.Length > 1)
                {
                    query += "&";
                }

                query += "nextToken=";
                query += nextToken;
            }
        }

        var msg = new HttpRequestMessage(HttpMethod.Get, new Uri(BaseAddress, url + query));
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        var response = await Client.SendAsync(msg);
        response.EnsureSuccessStatusCode();

        await using var body = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<QueuedResultResponse>(body, AlexaJsonOptions.DefaultOptions);
        return result!;
        
    }

    public async Task<CommandsResponse> Commands(CommandsRequest request)
    {
        // Serialize request using System.Text.Json
        var json = JsonSerializer.Serialize(request, AlexaJsonOptions.DefaultOptions);

        var msg = new HttpRequestMessage(HttpMethod.Post, new Uri(BaseAddress, "/v1/datastore/commands"))
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        var response = await Client.SendAsync(msg);
        response.EnsureSuccessStatusCode();

        await using var body = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<CommandsResponse>(body, AlexaJsonOptions.DefaultOptions);
        return result!;
    }
    public async Task<bool> Cancel(string queuedResultId)
    {
        var url = $"/v1/datastore/queue/{queuedResultId}/cancel";
        var msg = new HttpRequestMessage(HttpMethod.Post, new Uri(BaseAddress, url))
        {
            Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
        };
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        var response = await Client.SendAsync(msg);
        return response.StatusCode == HttpStatusCode.NoContent;
    }
}