using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class AnimatedTransform : AnimatedProperty
{
    public AnimatedTransform() { }

    public AnimatedTransform(APLTransform from, APLTransform to)
    {
        From = new List<APLTransform>
        {
            from
        };

        To = new List<APLTransform>
        {
            to
        };
    }

    [JsonPropertyName("property")]
    public override APLValue<string> Property => "transform";

    [JsonPropertyName("from")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLTransform>>? From { get; set; }

    [JsonPropertyName("to")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLTransform>>? To { get; set; }

    public static APLValue<IList<AnimatedProperty>> Multiple(IEnumerable<APLTransform> from, IEnumerable<APLTransform> to)
    {
        return new APLValue<IList<AnimatedProperty>>(
            new List<AnimatedProperty>
            {
                new AnimatedTransform
                {
                    From = new List<APLTransform>(from),
                    To = new List<APLTransform>(to)
                }
            });
    }

    public static APLValue<IList<AnimatedProperty>> Single(APLTransform from, APLTransform to)
    {
        return new APLValue<IList<AnimatedProperty>>(
            new List<AnimatedProperty>
            {
                new AnimatedTransform(from,to)
            });
    }
}