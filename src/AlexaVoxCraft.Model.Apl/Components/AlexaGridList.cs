﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaGridList : ResponsiveTemplate, IJsonSerializable<AlexaGridList>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaGridList);

    [JsonPropertyName("customLayoutName")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? CustomLayoutName { get; set; }

    [JsonPropertyName("defaultImnageSource")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? DefaultImnageSource { get; set; }

    [JsonPropertyName("imageAlignment")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AlexaImageAlignment?>? ImageAlignment { get; set; }

    [JsonPropertyName("imageAspectRatio")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AlexaImageAspectRatio?>? ImageAspectRatio { get; set; }

    [JsonPropertyName("imageBlurredBackground")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageBlurredBackground { get; set; }

    [JsonPropertyName("imageHideScrim")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageHideScrim { get; set; }

    [JsonPropertyName("imageMetadataPrimary")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageMetadataPrimary { get; set; }

    [JsonPropertyName("imageRoundedCorner")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageRoundedCorner { get; set; }

    [JsonPropertyName("imageScale")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Scale?>? Scale { get; set; }

    [JsonPropertyName("imageShowProgressBar")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageShowProgressBar { get; set; }

    [JsonPropertyName("listItemHeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? ListItemHeight { get; set; }

    [JsonPropertyName("listItemHorizontalCount")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int>? ListItemHorizontalCount { get; set; }

    [JsonPropertyName("listItems")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<object>>? ListItems { get; set; }

    [JsonPropertyName("primaryAction")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? PrimaryAction { get; set; }

    [JsonPropertyName("imageShadow")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageShadow { get; set; }

    [JsonPropertyName("headerAttributionOpacity")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? HeaderAttributionOpacity { get; set; }

    [JsonPropertyName("listId")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ListId { get; set; }

    [JsonPropertyName("speechItems")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SpeechItems { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaGridList
    {
        ResponsiveTemplate.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var listItemsProp = info.Properties.FirstOrDefault(p => p.Name == "listItems");
            if (listItemsProp is not null)
            {
                listItemsProp.CustomConverter = new GenericSingleOrListConverter<object>(false);
            }

            var primaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "primaryAction");
            if (primaryActionProp is not null)
            {
                primaryActionProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}