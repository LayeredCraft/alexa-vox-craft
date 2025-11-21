using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.Smapi.Clients;

/// <summary>
/// Base class for SMAPI HTTP clients providing common HTTP operations with JSON serialization.
/// </summary>
public abstract class BaseClient
{
    /// <summary>
    /// The HTTP client instance.
    /// </summary>
    protected readonly HttpClient Client;

    /// <summary>
    /// Optional JSON serializer options for request/response processing.
    /// </summary>
    protected readonly JsonSerializerOptions? JsonSerializerOptions;

    /// <summary>
    /// Logger instance for diagnostic logging.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseClient"/> class.
    /// </summary>
    /// <param name="client">The HTTP client instance.</param>
    /// <param name="logger">The logger instance.</param>
    public BaseClient(HttpClient client, ILogger logger)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    /// <summary>
    /// Sends a POST request and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The request URI.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response, or null if the response is empty.</returns>
    public virtual async Task<TResult?> PostAsync<TResult>(Uri uri, object? body = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        where TResult : class?
    {
        return await SendAsync<TResult>(uri, HttpMethod.Post, body, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request and returns the raw response string.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The raw response string.</returns>
    public virtual async Task<string?> PostStringAsync(Uri uri, object? body = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync(uri, HttpMethod.Post, body, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request without expecting a response body.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task PostAsync(Uri uri, object? body = null, Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        await SendAsync(uri, HttpMethod.Post, body, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PUT request and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The request URI.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response, or null if the response is empty.</returns>
    public virtual async Task<TResult?> PutAsync<TResult>(Uri uri, object? body = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        where TResult : class?
    {
        return await SendAsync<TResult>(uri, HttpMethod.Put, body, headers, cancellationToken);
    }

    /// <summary>
    /// Sends a PUT request and returns the raw response string.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The raw response string.</returns>
    public virtual async Task<string?> PutStringAsync(Uri uri, object? body = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync(uri, HttpMethod.Put, body, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PUT request without expecting a response body.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task PutAsync(Uri uri, object? body = null, Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        await SendAsync(uri, HttpMethod.Put, body, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PATCH request and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The request URI.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response, or null if the response is empty.</returns>
    public virtual async Task<TResult?> PatchAsync<TResult>(Uri uri, object? body = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        where TResult : class?
    {
        return await SendAsync<TResult>(uri, HttpMethod.Patch, body, headers, cancellationToken);
    }

    /// <summary>
    /// Sends a PATCH request and returns the raw response string.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The raw response string.</returns>
    public virtual async Task<string?> PatchStringAsync(Uri uri, object? body = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync(uri, HttpMethod.Patch, body, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PATCH request without expecting a response body.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task PatchAsync(Uri uri, object? body = null, Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        await SendAsync(uri, HttpMethod.Patch, body, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a GET request and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The request URI.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response, or null if the response is empty.</returns>
    public virtual async Task<TResult?> GetAsync<TResult>(Uri uri, Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default) where TResult : class?
    {
        return await SendAsync<TResult>(uri, HttpMethod.Get, null, headers, cancellationToken);
    }

    /// <summary>
    /// Sends a GET request and returns the raw response string.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The raw response string.</returns>
    public virtual async Task<string?> GetStringAsync(Uri uri,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync(uri, HttpMethod.Get, null, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The request URI.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response, or null if the response is empty.</returns>
    public virtual async Task<TResult?> DeleteAsync<TResult>(Uri uri, Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default) where TResult : class?
    {
        return await SendAsync<TResult>(uri, HttpMethod.Delete, null, headers, cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request and returns the raw response string.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The raw response string.</returns>
    public virtual async Task<string?> DeleteStringAsync(Uri uri,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync(uri, HttpMethod.Delete, null, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request without expecting a response body.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task DeleteAsync(Uri uri, object? body = null, Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        await SendAsync(uri, HttpMethod.Delete, body, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP request with the specified method and deserializes the response.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The request URI.</param>
    /// <param name="method">The HTTP method.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response, or null if the response is empty.</returns>
    public virtual async Task<TResult?> SendAsync<TResult>(Uri uri, HttpMethod method, object? body = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
        where TResult : class?
    {
        TResult? result = null;

        var rawResponse = await SendAsync(uri, method, body, headers, cancellationToken).ConfigureAwait(false);

        if (rawResponse != null)
        {
            result = JsonSerializer.Deserialize<TResult>(rawResponse, JsonSerializerOptions);
        }

        return result;
    }

    /// <summary>
    /// Sends an HTTP request with the specified method and returns the raw response string.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="method">The HTTP method.</param>
    /// <param name="body">Optional request body to serialize as JSON.</param>
    /// <param name="headers">Optional HTTP headers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The raw response string.</returns>
    public virtual async Task<string?> SendAsync(Uri uri, HttpMethod method, object? body = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        string? result = null;

        StringContent? content = null;
        if (body is not null)
            content = new StringContent(JsonSerializer.Serialize(body, JsonSerializerOptions), Encoding.UTF8,
                "application/json");

        var message = new HttpRequestMessage(method, uri)
        {
            Content = content
        };
        if (headers != null)
            foreach (var header in headers)
                message.Headers.Add(header.Key, header.Value);

        result = await SendAsync(message, cancellationToken).ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// Sends an HTTP request message and deserializes the response.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
    /// <param name="message">The HTTP request message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response, or null if the response is empty.</returns>
    public virtual async Task<TResult?> SendAsync<TResult>(HttpRequestMessage message,
        CancellationToken cancellationToken = default)
        where TResult : class?
    {
        TResult? result = null;

        var rawResponse = await SendAsync(message, cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(rawResponse))
        {
            result = JsonSerializer.Deserialize<TResult>(rawResponse, JsonSerializerOptions);
        }

        return result;
    }

    /// <summary>
    /// Sends an HTTP request message and returns the raw response string.
    /// Handles HTTP errors and logs diagnostic information.
    /// </summary>
    /// <param name="message">The HTTP request message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The raw response string, or null if the resource was not found.</returns>
    public  async Task<string?> SendAsync(HttpRequestMessage message, CancellationToken cancellationToken = default)
    {
        string? result = null;

        try
        {
            using var response = await Client.SendAsync(message, cancellationToken);

            response.EnsureSuccessStatusCode();

            result = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            Logger.Debug("Raw response from {Uri} was {RawResponse}", message.RequestUri,
                propertyValue1: result);

            return result;
        }
        catch (HttpRequestException ex)
        {
            Logger.Error(ex, "Http status {Status}", ex.StatusCode);
            if (ex.StatusCode == HttpStatusCode.NotFound)
                return result;
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to call {Uri}", message.RequestUri);
            throw;
        }
    }
}