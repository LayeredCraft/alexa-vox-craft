using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.SmartMotion;

public class TurnToPrimaryUserCommand : APLCommand
{
    private string _extensionName;

    public static TurnToPrimaryUserCommand For(SmartMotionExtension extension)
    {
        return new TurnToPrimaryUserCommand(extension.Name);
    }

    public TurnToPrimaryUserCommand(string extensionName)
    {
        _extensionName = extensionName;
    }

    [JsonPropertyName("type")] public override string Type => $"{_extensionName}:TurnToPrimaryUser";
}