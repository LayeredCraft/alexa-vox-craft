using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public abstract class ResponsiveTemplate : APLComponent, IJsonSerializable<ResponsiveTemplate>
{
    [JsonPropertyName("backgroundAlign")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? BackgroundAlign { get; set; }

    [JsonPropertyName("backgroundBlur")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? BackgroundBlur { get; set; }

    [JsonPropertyName("backgroundColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? BackgroundColor { get; set; }

    [JsonPropertyName("backgroundColorOverlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? BackgroundColorOverlay { get; set; }

    [JsonPropertyName("backgroundOverlayGradient")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? BackgroundOverlayGradient { get; set; }

    [JsonPropertyName("backgroundImageSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? BackgroundImageSource { get; set; }

    [JsonPropertyName("backgroundScale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Scale?>? BackgroundScale { get; set; }

    [JsonPropertyName("backgroundVideoSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<VideoSource>? BackgroundVideoSource { get; set; }

    [JsonPropertyName("backgroundVideoAudioTrack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? BackgroundVideoAudioTrack { get; set; }

    [JsonPropertyName("backgroundVideoAutoPlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? BackgroundVideoAutoPlay { get; set; }

    [JsonPropertyName("headerDivider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HeaderDivider { get; set; }

    [JsonPropertyName("headerTitle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HeaderTitle { get; set; }

    [JsonPropertyName("headerSubtitle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HeaderSubtitle { get; set; }

    [JsonPropertyName("headerAttributionText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HeaderAttributionText { get; set; }

    [JsonPropertyName("headerAttributionImage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HeaderAttributionImage { get; set; }

    [JsonPropertyName("headerAttributionPrimacy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HeaderAttributionPrimacy { get; set; }

    [JsonPropertyName("headerBackButton")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HeaderBackButton { get; set; }

    [JsonPropertyName("headerBackButtonAccessibilityLabel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HeaderBackButtonAccessibilityLabel { get; set; }

    [JsonPropertyName("headerBackButtonCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? HeaderBackButtonCommand { get; set; }

    [JsonPropertyName("headerBackgroundColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HeaderBackgroundColor { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("footerHintText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? FooterHintText { get; set; }

    public new static void RegisterTypeInfo<T>() where T : ResponsiveTemplate
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}