using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Http.Clients;
using AlexaVoxCraft.Smapi.Models.Invocation;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.Smapi.Clients;

/// <summary>
/// Client for invoking Alexa skills through the SMAPI Skill Invocation API.
/// </summary>
public sealed class AlexaSkillInvocationClient : BaseClient, IAlexaSkillInvocationClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlexaSkillInvocationClient"/> class.
    /// </summary>
    /// <param name="client">The configured HTTP client.</param>
    /// <param name="logger">The logger instance.</param>
    public AlexaSkillInvocationClient(
        HttpClient client,
        ILogger<AlexaSkillInvocationClient> logger) : base(client, logger, new JsonSerializerOptions
    {
        PropertyNamingPolicy = null, // 👈 important
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    })
    {
    }

    /// <inheritdoc />
    public async Task<SkillInvocationResponse<TResponse>?> InvokeAsync<TRequest, TResponse>(
        string skillId,
        string stage,
        TRequest skillRequest,
        InvocationRegion endpointRegion = InvocationRegion.Default,
        CancellationToken ct = default)
    {
        return await PostAsync<SkillInvocationResponse<TResponse>>(
            new Uri(SkillInvocationEndpoints.Invoke(skillId, stage), UriKind.Relative),
            new SkillInvocationRequest<TRequest>
            {
                EndpointRegion = endpointRegion,
                SkillRequest = new SkillInvocationBody<TRequest>
                {
                    Body = skillRequest
                }
            }, null, ct).ConfigureAwait(false);
    }

    private static class SkillInvocationEndpoints
    {
        public static string Invoke(string skillId, string stage) =>
            $"/v2/skills/{skillId}/stages/{stage}/invocations";
    }
}