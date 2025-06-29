using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Helpers;

namespace AlexaVoxCraft.Model.ConnectionTasks.Inputs;

public class ScheduleTaxiReservation : IConnectionTask
{
    public const string ConnectionType = "ScheduleTaxiReservationRequest";
    public const string VersionNumber = "1";
    public const string ConnectionKey = $"{ConnectionType}/{VersionNumber}";

    public const string AssociatedUri = "connection://AMAZON.ScheduleTaxiReservation/1";
    [JsonIgnore] public string ConnectionUri => AssociatedUri;

    [JsonPropertyName("@type")] public string Type => ConnectionType;

    [JsonPropertyName("@version")] public string Version => VersionNumber;

    [JsonPropertyName("context")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public ConnectionTaskContext Context { get; set; }

    [JsonPropertyName("partySize")] public int PartySize { get; set; }
    [JsonPropertyName("pickupLocation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PostalAddress PickupLocation { get; set; }

    [JsonPropertyName("dropoffLocation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PostalAddress DropoffLocation { get; set; }

    [JsonPropertyName("pickupTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(MixedDateTimeConverter))]
    public DateTime? PickupTime { get; set; }
}