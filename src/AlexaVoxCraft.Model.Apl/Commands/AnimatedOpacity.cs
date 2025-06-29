using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class AnimatedOpacity : AnimatedProperty
{
    public AnimatedOpacity() { }

    public AnimatedOpacity(APLValue<double?> from, APLValue<double?> to)
    {
        From = from;
        To = to;
    }

    [JsonPropertyName("property")]
    public override APLValue<string> Property => "opacity";

    [JsonPropertyName("from")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? From { get; set; }

    [JsonPropertyName("to")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? To { get; set; }

    public static APLValue<IList<AnimatedProperty>> Single(double? from, double? to)
    {
        return new APLValue<IList<AnimatedProperty>>(
            new List<AnimatedProperty>
            {
                new AnimatedOpacity(from,to)
            });
    }
}