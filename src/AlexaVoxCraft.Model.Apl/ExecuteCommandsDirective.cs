using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Converters;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class ExecuteCommandsDirective : IDirective, IJsonSerializable<ExecuteCommandsDirective>
{
    private const string DirectiveType = "Alexa.Presentation.APL.ExecuteCommands";

    public static void AddSupport()
    {
        DirectiveConverter.RegisterDirectiveDerivedType<ExecuteCommandsDirective>(DirectiveType);
    }

    public ExecuteCommandsDirective()
    {
    }

    public ExecuteCommandsDirective(string token)
    {
        Token = token;
    }

    public ExecuteCommandsDirective(string token, IEnumerable<APLCommand> commands) :
        this(token)
    {
        Commands = [.. commands];
    }

    public ExecuteCommandsDirective(string token, params APLCommand[] commands) :
        this(token, (IEnumerable<APLCommand>)commands)
    {
    }

    [JsonPropertyName("type")] public string Type => DirectiveType;

    [JsonPropertyName("token")] public string Token { get; set; }

    [JsonPropertyName("commands")]
    public IList<APLCommand> Commands { get; set; }

    public static void RegisterTypeInfo<T>() where T : ExecuteCommandsDirective
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var commandsProp = info.Properties.FirstOrDefault(p => p.Name == "commands");
            commandsProp?.CustomConverter = new APLCommandListConverter(true);
        });

    }
}