using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Helpers;

namespace AlexaVoxCraft.Model.ConnectionTasks.Inputs;

public class ScheduleFoodEstablishmentReservation:IConnectionTask
{
    public const string ConnectionType = "ScheduleFoodEstablishmentReservationRequest";
    public const string VersionNumber = "1";
    public const string ConnectionKey = $"{ConnectionType}/{VersionNumber}";

    public const string AssociatedUri = "connection://AMAZON.ScheduleFoodEstablishmentReservation/1";
    [JsonIgnore]
    public string ConnectionUri => AssociatedUri;

    [JsonPropertyName("@type")] public string Type => ConnectionType;

    [JsonPropertyName("@version")] public string Version => VersionNumber;

    [JsonPropertyName("context")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConnectionTaskContext Context { get; set; }

    [JsonPropertyName("partySize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PartySize { get; set; }

    [JsonPropertyName("startTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(MixedDateTimeConverter))]
    public DateTime? StartTime { get; set; }

    [JsonPropertyName("restaurant")]
    public Restaurant Restaurant { get; set; }
}