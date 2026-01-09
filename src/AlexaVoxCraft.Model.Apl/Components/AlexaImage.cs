using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaImage : APLComponent, IJsonSerializable<AlexaImage>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaImage);

    [JsonPropertyName("imageAlignment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AlexaImageAlignment?>? ImageAlignment { get; set; }

    [JsonPropertyName("imageAspectRatio")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AlexaImageAspectRatio?>? ImageAspectRatio { get; set; }

    [JsonPropertyName("imageBlurredBackground")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageBlurredBackground { get; set; }

    [JsonPropertyName("imageHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? ImageHeight { get; set; }

    [JsonPropertyName("imageWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? ImageWidth { get; set; }

    [JsonPropertyName("imageRoundedCorner")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageRoundedCorner { get; set; }

    [JsonPropertyName("imageScale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Scale?>? Scale { get; set; }

    [JsonPropertyName("imageSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ImageSource { get; set; }

    [JsonPropertyName("imageShadow")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageShadow { get; set; }

    [JsonPropertyName("onLoad")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? OnLoad { get; set; }

    [JsonPropertyName("onFail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? OnFail { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaImage
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onLoadProp = info.Properties.FirstOrDefault(p => p.Name == "onLoad");
            if (onLoadProp is not null)
            {
                onLoadProp.CustomConverter = new APLCommandListConverter(true);
            }

            var onFailProp = info.Properties.FirstOrDefault(p => p.Name == "onFail");
            if (onFailProp is not null)
            {
                onFailProp.CustomConverter = new APLCommandListConverter(true);
            }
        });
    }
}