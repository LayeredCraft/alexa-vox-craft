using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Responses.Apl.Commands;

namespace AlexaVoxCraft.Model.Responses.Apl.Components;

public abstract class AlexaSliderBase : TouchComponent
{
        [JsonPropertyName("bufferValue"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<int?> BufferValue { get; set; }

        [JsonPropertyName("isLoading"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<bool?> IsLoading { get; set; }

        [JsonPropertyName("progressValue"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<int?> ProgressValue { get; set; }

        [JsonPropertyName("theme"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<string> Theme { get; set; }

        [JsonPropertyName("totalValue"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<int?> TotalValue { get; set; }

        [JsonPropertyName("thumbDisplayedAllStates"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<bool?> ThumbDisplayedAllStates { get; set; }

        [JsonPropertyName("thumbColor"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<string> ThumbColor { get; set; }

        [JsonPropertyName("progressFillColor"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<string> ProgressFillColor { get; set; }

        [JsonPropertyName("positionPropertyName"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<string> PositionPropertyName { get; set; }

        [JsonPropertyName("onBlurCommand"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<IEnumerable<APLCommand>> OnBlurCommand { get; set; }

        [JsonPropertyName("onUpCommand"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<IEnumerable<APLCommand>> OnUpCommand { get; set; }

        [JsonPropertyName("onDownCommand"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<IEnumerable<APLCommand>> OnDownCommand { get; set; }

        [JsonPropertyName("onFocusCommand"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<IEnumerable<APLCommand>> OnFocusCommand { get; set; }

        [JsonPropertyName("onMoveCommand"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<IEnumerable<APLCommand>> OnMoveCommand { get; set; }

        [JsonPropertyName("handleKeyDownCommand"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<IEnumerable<APLCommand>> HandleKeyDownCommand { get; set; }

        [JsonPropertyName("metadataDisplayed"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public APLValue<bool> MetadataDisplayed { get; set; }
}