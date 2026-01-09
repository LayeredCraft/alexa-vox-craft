using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class AnimatedTransform : AnimatedProperty
{
    public AnimatedTransform()
    {
    }

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

    [JsonPropertyName("property")] public override APLValue<string>? Property => "transform";

    [JsonPropertyName("from")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLTransform>? From { get; set; }

    [JsonPropertyName("to")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLTransform>? To { get; set; }

    public static APLValueCollection<AnimatedProperty> Multiple(IEnumerable<APLTransform> from,
        IEnumerable<APLTransform> to) =>
    [
        new AnimatedTransform
        {
            From = [..from],
            To = [..to]
        }
    ];

    public static APLValueCollection<AnimatedProperty> Single(APLTransform from, APLTransform to) =>
        [new AnimatedTransform(from, to)];

}