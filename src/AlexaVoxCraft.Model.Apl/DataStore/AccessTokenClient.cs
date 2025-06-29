using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class AccessTokenClient
{
    public const string ApiDomainBaseAddress = "https://api.amazon.com";
    public const string DataStoreScope = "alexa::datastore";
    public const string ClientCredentials = "client_credentials";

    public HttpClient Client { get; set; }
    private string BaseAddress { get; }

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public AccessTokenClient() : this(new HttpClient(), null) { }

    public AccessTokenClient(string baseAddress) : this(null, baseAddress) { }

    public AccessTokenClient(HttpClient client) : this(client, null) { }

    public AccessTokenClient(HttpClient? client, string? baseAddress)
    {
        BaseAddress = baseAddress ?? ApiDomainBaseAddress;
        Client = client ?? new HttpClient();
    }

    public async Task<AccessToken> Send(string clientId, string clientSecret)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "grant_type", ClientCredentials },
            { "scope", DataStoreScope }
        });

        var msg = new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(BaseAddress), "/auth/O2/token"))
        {
            Content = content
        };

        var response = await Client.SendAsync(msg);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<AccessToken>(stream, Options)
               ?? throw new InvalidOperationException("Failed to deserialize access token.");
    }
}

public class AccessToken
{
    [JsonPropertyName("access_token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}