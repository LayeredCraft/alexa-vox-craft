using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Unit
{
    [JsonPropertyName("unitId")]
    public string UnitID { get; set; } = null!;

    [JsonPropertyName("persistentUnitId")]
    public string PersistentUnitID { get; set; } = null!;
}