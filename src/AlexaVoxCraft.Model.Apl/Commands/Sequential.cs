using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

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
    public APLValueCollection<APLCommand>? Finally { get; set; }

    [JsonPropertyName("catch")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? Catch { get; set; }

    [JsonPropertyName("commands")]
    public APLValueCollection<APLCommand> Commands { get; set; }

    [JsonPropertyName("repeatCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? RepeatCount { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object>? Data { get; set; }
}