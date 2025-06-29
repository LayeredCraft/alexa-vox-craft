using System.Collections.Generic;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Audio;

namespace AlexaVoxCraft.Model.Apl;

public class APLADocument : APLDocumentReference
{
    public const string DocumentType = "APLA";

    public APLADocument()
    {
        VersionString = "0.8";
    }

    [JsonPropertyName("type")]
    public override string Type => DocumentType;

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }

    [JsonIgnore]
    public APLADocumentVersion Version
    {
        get => EnumParser.ToEnum(VersionString, APLADocumentVersion.Unknown);
        set => VersionString = EnumParser.ToEnumString(typeof(APLADocumentVersion), value);
    }

    [JsonPropertyName("mainTemplate")] public AudioLayout MainTemplate { get; set; }

    [JsonPropertyName("resources")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<APLAResource>? Resources { get; set; }

    [JsonPropertyName("compositions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, AudioLayout>? Compositions { get; set; }
}