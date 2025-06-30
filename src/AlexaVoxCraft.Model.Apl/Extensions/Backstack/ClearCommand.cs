using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.Backstack;

public class ClearCommand : APLCommand
{
    private readonly string _extensionName;

    public static ClearCommand For(BackstackExtension extension)
    {
        return new ClearCommand(extension.Name);
    }

    public ClearCommand(string extensionName)
    {
        _extensionName = extensionName;
    }

    [JsonPropertyName("type")]
    public override string Type => $"{_extensionName}:Clear";
}