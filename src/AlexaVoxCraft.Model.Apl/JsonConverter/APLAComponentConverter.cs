using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using AlexaVoxCraft.Model.Apl.Audio;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLAComponentConverter: BasePolymorphicConverter<APLAComponent>
{
    private static IDictionary<string, Type> _derivedTypes = new Dictionary<string, Type>
    {
        { nameof(Audio), typeof(Audio.Audio) },
        { nameof(Speech), typeof(Speech) },
        { nameof(Sequencer), typeof(Sequencer) },
        { nameof(Selector), typeof(Selector) },
        { nameof(Mixer), typeof(Mixer) },
    };

    public override void Write(Utf8JsonWriter writer, APLAComponent value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
    
    protected override JsonElement TransformJson(JsonElement original)
    {
        var node = JsonNode.Parse(original.GetRawText())!.AsObject();

        if (node.TryGetPropertyValue("item", out var itemNode))
        {
            node.Remove("item");
            node["items"] = itemNode;
        }

        return JsonSerializer.SerializeToElement(node);
    }

    protected override string TypeDiscriminatorPropertyName => "type";
    protected override IDictionary<string, Type> DerivedTypes => _derivedTypes;

    protected override IDictionary<string, Func<JsonElement, Type>> DataDrivenTypeFactories =>
        new Dictionary<string, Func<JsonElement, Type>>();

    protected override Func<JsonElement, Type?>? CustomTypeResolver => null;
    protected override Type? DefaultType => null;
}