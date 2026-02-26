using System.Text.Json.Serialization;

namespace AlexaVoxCraft.InSkillPurchasing.Models;

/// <summary>
/// Represents a paginated response from the in-skill product transactions API.
/// </summary>
public sealed record TransactionResponse([property: JsonPropertyName("results")] Transaction[] Results,
    [property: JsonPropertyName("metadata")] TransactionResponseMetadata? Metadata = null);

/// <summary>
/// Represents a single in-skill purchase transaction.
/// </summary>
public sealed record Transaction(
    [property: JsonPropertyName("status"), JsonConverter(typeof(JsonStringEnumConverter))]
    TransactionStatus Status,
    [property: JsonPropertyName("productId")]
    string ProductId,
    [property: JsonPropertyName("createdTime")]
    DateTimeOffset CreatedTime,
    [property: JsonPropertyName("lastModifiedTime")]
    DateTimeOffset LastModifiedTime);
    
/// <summary>
/// Contains metadata associated with a transaction list response, such as pagination info.
/// </summary>
public sealed record TransactionResponseMetadata([property: JsonPropertyName("resultSet")] TransactionResultSet ResultSet);

/// <summary>
/// Holds the pagination token for fetching the next page of transaction results.
/// </summary>
public sealed record TransactionResultSet([property: JsonPropertyName("nextToken")] string NextToken);

/// <summary>
/// Defines optional filter criteria for querying in-skill purchase transactions.
/// </summary>
public sealed record TransactionFilter(
    string? ProductId = null,
    string? NextToken = null,
    int? MaxResults = null,
    TransactionStatus? Status = null,
    DateTimeOffset? FromModifiedDateTime = null,
    DateTimeOffset? ToModifiedDateTime = null);

/// <summary>
/// Extension methods for building query strings from a <see cref="TransactionFilter"/>.
/// </summary>
public static class TransactionExtensions
{
    extension(TransactionFilter? filter)
    {
        public bool AnyFilters =>
            !string.IsNullOrWhiteSpace(filter?.ProductId) || !string.IsNullOrWhiteSpace(filter?.NextToken) ||
            filter?.MaxResults.HasValue == true || filter?.Status.HasValue == true ||
            filter?.FromModifiedDateTime.HasValue == true || filter?.ToModifiedDateTime.HasValue == true;

        public string ToQueryString()
        {
            var query = string.Empty;
            if (!(filter?.AnyFilters ?? false)) return query;
            var content = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(filter.ProductId))
            {
                content.Add("productId", filter.ProductId!);
            }

            if (!string.IsNullOrWhiteSpace(filter.NextToken))
            {
                content.Add("nextToken", filter.NextToken!);
            }

            if (filter.Status.HasValue)
            {
                content.Add("status", filter.Status.Value.ToString());
            }

            if (filter.MaxResults.HasValue)
            {
                content.Add("maxResults", filter.MaxResults.Value.ToString());
            }

            if (filter.FromModifiedDateTime.HasValue)
            {
                content.Add("fromLastModifiedTime", filter.FromModifiedDateTime.Value.ToString("O"));
            }

            if (filter.ToModifiedDateTime.HasValue)
            {
                content.Add("toLastModifiedTime", filter.ToModifiedDateTime.Value.ToString("O"));
            }

            query = "?" + string.Join("&", content.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            return query;
        }
    }
}

/// <summary>
/// Specifies the status of an in-skill purchase transaction.
/// </summary>
public enum TransactionStatus
{
    PENDING_APPROVAL_BY_PARENT,
    APPROVED_BY_PARENT,
    DENIED_BY_PARENT,
    EXPIRED_NO_ACTION_BY_PARENT,
    ERROR
}