using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.SmartMotion;

public class PlayNamedChoreoCommand : APLCommand
{
    private string _extensionName;

    public static PlayNamedChoreoCommand For(SmartMotionExtension extension, string choreoName = null)
    {
        return new PlayNamedChoreoCommand(extension.Name, choreoName);
    }

    public PlayNamedChoreoCommand(string extensionName, string choreoName = null)
    {
        _extensionName = extensionName;
        Name = choreoName;
    }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Name { get; set; }

    [JsonPropertyName("type")] public override string Type => $"{_extensionName}:PlayNamedChoreo";
}