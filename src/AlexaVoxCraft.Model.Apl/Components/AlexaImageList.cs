using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaImageList : AlexaImageListBase, IJsonSerializable<AlexaImageList>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaImageList);

    [JsonPropertyName("defaultImageSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? DefaultImageSource { get; set; }

    [JsonPropertyName("listItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<AlexaImageListItem>>? ListItems { get; set; }

    [JsonPropertyName("videoAudioTrack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? VideoAudioTrack { get; set; }

    [JsonPropertyName("videoAutoPlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? VideoAutoPlay { get; set; }

    [JsonPropertyName("lang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Lang { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaImageList
    {
        AlexaImageListBase.RegisterTypeInfo<T>();
    }
}