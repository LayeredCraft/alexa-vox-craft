using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.Backstack;

public class GoBackCommand : APLCommand
{
    private readonly string _extensionName;

    public static GoBackCommand For(BackstackExtension extension)
    {
        return new GoBackCommand(extension.Name);
    }

    public GoBackCommand(string extensionName)
    {
        _extensionName = extensionName;
    }

    [JsonPropertyName("type")]
    public override string Type => $"{_extensionName}:GoBack";

    [JsonPropertyName("backType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BackTypeKind? BackType { get; set; }

    [JsonPropertyName("backValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object BackValue { get; set; }
}