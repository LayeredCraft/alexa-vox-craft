using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Responses.Apl.Commands;

namespace AlexaVoxCraft.Model.Responses.Apl.Components;

public class AlexaGridList : ResponsiveTemplate
{
        [JsonPropertyName("customLayoutName"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<string> CustomLayoutName { get; set; }

        [JsonPropertyName("defaultImnageSource"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<string> DefaultImnageSource { get; set; }

        [JsonPropertyName("imageAlignment"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<AlexaImageAlignment?> ImageAlignment { get; set; }

        [JsonPropertyName("imageAspectRatio"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<AlexaImageAspectRatio?> ImageAspectRatio { get; set; }

        [JsonPropertyName("imageBlurredBackground"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<bool?> ImageBlurredBackground { get; set; }

        [JsonPropertyName("imageHideScrim"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<bool?> ImageHideScrim { get; set; }

        [JsonPropertyName("imageMetadataPrimary"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<bool?> ImageMetadataPrimary { get; set; }

        [JsonPropertyName("imageRoundedCorner"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<bool?> ImageRoundedCorner { get; set; }

        [JsonPropertyName("imageScale"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<Scale?> Scale { get; set; }

        [JsonPropertyName("imageShowProgressBar"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<bool?> ImageShowProgressBar { get; set; }

        [JsonPropertyName("listItemHeight"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLDimensionValue ListItemHeight { get; set; }

        [JsonPropertyName("listItemHorizontalCount"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<int> ListItemHorizontalCount { get; set; }

        [JsonPropertyName("listItems"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<IEnumerable<object>> ListItems { get; set; }

        [JsonPropertyName("primaryAction"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<IEnumerable<APLCommand>> PrimaryAction { get; set; }

        [JsonPropertyName("imageShadow"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<bool?> ImageShadow { get; set; }

        [JsonPropertyName("headerAttributionOpacity"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<double?> HeaderAttributionOpacity { get; set; }

        [JsonPropertyName("listId"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<string> ListId { get; set; }

        [JsonPropertyName("speechItems"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<string> SpeechItems { get; set; }
}