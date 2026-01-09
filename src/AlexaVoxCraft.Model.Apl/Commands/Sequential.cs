using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Sequential : APLCommand
{
    public Sequential()
    {
    }

    public Sequential(IEnumerable<APLCommand> commands)
    {
        Commands = commands.ToList();
    }

    public Sequential(params APLCommand[] commands) : this((IEnumerable<APLCommand>)commands)
    {
    }

    public override string Type => nameof(Sequential);

    [JsonPropertyName("finally")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? Finally { get; set; }

    [JsonPropertyName("catch")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? Catch { get; set; }

    [JsonPropertyName("commands")]
    public APLValueCollection<APLCommand>> Commands { get; set; }

    [JsonPropertyName("repeatCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? RepeatCount { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object>>? Data { get; set; }

    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<Sequential>(info =>
        {
            var finallyProp = info.Properties.FirstOrDefault(p => p.Name == "finally");
            if (finallyProp is not null)
            {
                finallyProp.CustomConverter = new APLCommandListConverter(false);
            }
            var catchProp = info.Properties.FirstOrDefault(p => p.Name == "catch");
            if (catchProp is not null)
            {
                catchProp.CustomConverter = new APLCommandListConverter(false);
            }
            var commandsProp = info.Properties.FirstOrDefault(p => p.Name == "commands");
            if (commandsProp is not null)
            {
                commandsProp.CustomConverter = new APLCommandListConverter(false);
            }
            var dataProp = info.Properties.FirstOrDefault(p => p.Name == "data");
            if (dataProp is not null)
            {
                dataProp.CustomConverter = new GenericSingleOrListConverter<object>(false);
            }
        });
    }
}