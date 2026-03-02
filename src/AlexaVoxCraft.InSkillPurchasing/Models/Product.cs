using System.Text.Json.Serialization;

namespace AlexaVoxCraft.InSkillPurchasing.Models;

/// <summary>
/// Represents a paginated response from the in-skill products API.
/// </summary>
public sealed record ProductResponse([property: JsonPropertyName("inSkillProducts")] Product[] Products,
    [property: JsonPropertyName("isTruncated")] bool IsTruncated,
    [property: JsonPropertyName("nextToken")] string? NextToken = null);

/// <summary>
/// Represents an in-skill product that can be purchased by a customer.
/// </summary>
public sealed record Product(
    [property: JsonPropertyName("productId")] string ProductId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type"), JsonConverter(typeof(JsonStringEnumConverter))] ProductType Type,
    [property: JsonPropertyName("summary")] string Summary,
    [property: JsonPropertyName("purchasable"), JsonConverter(typeof(JsonStringEnumConverter))] Purchasable Purchasable,
    [property: JsonPropertyName("entitled"), JsonConverter(typeof(JsonStringEnumConverter))] Entitled Entitled,
    [property: JsonPropertyName("entitlementReason"), JsonConverter(typeof(JsonStringEnumConverter))] EntitlementReason EntitlementReason,
    [property: JsonPropertyName("referenceName")] string ReferenceName,
    [property: JsonPropertyName("activeEntitlementCount")] int ActiveEntitlementCount,
    [property: JsonPropertyName("purchaseMode"), JsonConverter(typeof(JsonStringEnumConverter))] PurchaseMode PurchaseMode)
{
    [JsonPropertyName("startTime")] public DateTimeOffset StartTime { get; set; }

    [JsonPropertyName("endTime")] public DateTimeOffset EndTime { get; set; }
}

/// <summary>
/// Defines optional filter criteria for querying in-skill products.
/// </summary>
public sealed record ProductFilter(
    int? MaxResults = null,
    Entitled? Entitled = null,
    Purchasable? Purchasable = null,
    ProductType? ProductType = null);

/// <summary>
/// Extension methods for building query strings from a <see cref="ProductFilter"/>.
/// </summary>
public static class ProductFilterExtensions
{
    extension(ProductFilter? filter)
    {
        public bool AnyFilters => filter != null && (filter.MaxResults.HasValue || filter.Entitled.HasValue ||
                                                     filter.Purchasable.HasValue || filter.ProductType.HasValue);

        public string ToQueryString()
        {
            var query = string.Empty;
            if (!(filter?.AnyFilters ?? false)) return query;
            if (filter.MaxResults is < 0 or > 100)
            {
                throw new InvalidOperationException("When set MaxResults must be between 1 and 100");
            }

            var content = new Dictionary<string, string>();

            if (filter.Purchasable.HasValue)
            {
                content.Add("purchasable", filter.Purchasable.ToString()!);
            }

            if (filter.Entitled.HasValue)
            {
                content.Add("entitled", filter.Entitled.ToString()!);
            }

            if (filter.ProductType.HasValue)
            {
                content.Add("productType", filter.ProductType.ToString()!);
            }

            if (filter.MaxResults.HasValue)
            {
                content.Add("maxResults", filter.MaxResults.ToString()!);
            }


            query = "?" + string.Join("&", content.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            return  query;
        }
    }
}

/// <summary>
/// Specifies the type of an in-skill product.
/// </summary>
public enum ProductType
{
    CONSUMABLE,
    ENTITLEMENT,
    SUBSCRIPTION
}

/// <summary>
/// Specifies whether an in-skill product is available for purchase.
/// </summary>
public enum Purchasable
{
    NOT_PURCHASABLE,
    PURCHASABLE
}

/// <summary>
/// Specifies whether the customer is entitled to an in-skill product.
/// </summary>
public enum Entitled
{
    NOT_ENTITLED,
    ENTITLED
}

/// <summary>
/// Specifies the reason a customer is entitled to an in-skill product.
/// </summary>
public enum EntitlementReason
{
    PURCHASED,
    NOT_PURCHASED,
    AUTO_ENTITLED
}

/// <summary>
/// Specifies the purchasing mode for an in-skill product.
/// </summary>
public enum PurchaseMode
{
    LIVE,
    TEST
}