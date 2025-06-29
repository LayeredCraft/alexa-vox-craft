using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Helpers;

namespace AlexaVoxCraft.Model.Apl;

public class APLInterfaceRuntime
{
    [JsonIgnore]
    public APLDocumentVersion MaxVersion
    {
        get => ToEnum(MaxVersionString);
        set => MaxVersionString = EnumHelper.ToEnumString(value);
    }

    private static APLDocumentVersion ToEnum(string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return APLDocumentVersion.Unknown;
        }

        return EnumHelper.TryParseEnumWithEnumMemberSupport<APLDocumentVersion>(str, out var parsed)
            ? parsed
            : APLDocumentVersion.Unknown;
    }


    [JsonPropertyName("maxVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MaxVersionString { get; set; }
}