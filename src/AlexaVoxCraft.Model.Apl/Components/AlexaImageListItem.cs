using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaImageListItem : AlexaPaginatedListItem, IJsonSerializable<AlexaImageListItem>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaImageListItem);

    [JsonPropertyName("defaultImageSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? DefaultImageSource { get; set; }

    [JsonPropertyName("imageAlignment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AlexaImageAlignment?>? ImageAlignment { get; set; }

    [JsonPropertyName("imageAspectRatio")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AlexaImageAspectRatio?>? ImageAspectRatio { get; set; }

    [JsonPropertyName("imageBlurredBackground")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageBlurredBackground { get; set; }

    [JsonPropertyName("imageHideScim")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageHideScrim { get; set; }

    [JsonPropertyName("imageMetadataPrimary")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageMetadataPrimary { get; set; }

    [JsonPropertyName("imageProgressBarPercentage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? ImageProgressBarPercentage { get; set; }

    [JsonPropertyName("imageRoundedCorner")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageRoundedCorner { get; set; }

    [JsonPropertyName("imageScale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Scale?>? ImageScale { get; set; }

    [JsonPropertyName("providerText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ProviderText { get; set; }

    [JsonPropertyName("hasPlayIcon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HasPlayIcon { get; set; }

    [JsonPropertyName("imageSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ImageSource { get; set; }

    [JsonPropertyName("imageShadow")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageShadow { get; set; }

    [JsonPropertyName("contentDirection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ContentDirection?>? ContentDirection { get; set; }

    [JsonPropertyName("imageAltText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ImageAltText { get; set; }

    public static void RegisterTypeInfo<T>() where T : AlexaImageListItem
    {
        AlexaPaginatedListItem.RegisterTypeInfo<T>();
    }
}