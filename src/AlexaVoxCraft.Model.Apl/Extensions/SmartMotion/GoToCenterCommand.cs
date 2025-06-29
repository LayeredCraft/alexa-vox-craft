using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.SmartMotion;

public class GoToCenterCommand : APLCommand
{
    private string _extensionName;

    public static GoToCenterCommand For(SmartMotionExtension extension)
    {
        return new GoToCenterCommand(extension.Name);
    }

    public GoToCenterCommand(string extensionName)
    {
        _extensionName = extensionName;
    }

    [JsonPropertyName("type")] public override string Type => $"{_extensionName}:GoToCenter";
}