using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Parallel : APLCommand, IJsonSerializable<Parallel>
{
    public Parallel()
    {
    }

    public Parallel(IEnumerable<APLCommand> commands)
    {
        Commands = commands.ToList();
    }

    public Parallel(params APLCommand[] commands) : this((IEnumerable<APLCommand>)commands)
    {
    }

    public override string Type => nameof(Parallel);

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? Commands { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object> Data { get; set; }

    public static void RegisterTypeInfo<T>() where T : Parallel
    {
        AlexaJsonOptions.RegisterTypeModifier<Parallel>(info =>
        {
            var commandsProp = info.Properties.FirstOrDefault(p => p.Name == "commands");
            commandsProp?.CustomConverter = new APLCommandListConverter(false);
            var dataProp = info.Properties.FirstOrDefault(p => p.Name == "data");
            dataProp?.CustomConverter = new APLValueCollectionConverter<object>(false);
        });
    }
}