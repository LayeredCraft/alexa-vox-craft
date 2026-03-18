using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.Invocation;

public sealed record SkillInvocationResponse<TResponse>
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("result")]
    public SkillInvocationResult<TResponse>? Result { get; init; }
}

public sealed record SkillInvocationResult<TResponse>
{
    [JsonPropertyName("skillExecutionInfo")]
    public SkillExecutionInfo<TResponse>? SkillExecutionInfo { get; init; }

    [JsonPropertyName("error")]
    public SkillInvocationError? Error { get; init; }
}

public sealed record SkillExecutionInfo<TResponse>
{
    [JsonPropertyName("invocationRequest")]
    public InvocationRequestInfo? InvocationRequest { get; init; }

    [JsonPropertyName("invocationResponse")]
    public InvocationResponseInfo<TResponse>? InvocationResponse { get; init; }

    [JsonPropertyName("metrics")]
    public SkillExecutionMetrics? Metrics { get; init; }
}

public sealed record InvocationRequestInfo
{
    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; init; }

    [JsonPropertyName("body")]
    public object? Body { get; init; }
}

public sealed record InvocationResponseInfo<TResponse>
{
    [JsonPropertyName("body")]
    public TResponse? Body { get; init; }
}

public sealed record SkillExecutionMetrics
{
    [JsonPropertyName("skillExecutionTimeInMilliseconds")]
    public int SkillExecutionTimeInMilliseconds { get; init; }
}

public sealed record SkillInvocationError
{
    [JsonPropertyName("message")]
    public string? Message { get; init; }
}