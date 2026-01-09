using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Parallel : APLCommand
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
}