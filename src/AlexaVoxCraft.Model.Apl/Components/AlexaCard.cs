using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaCard : ResponsiveTemplate, IJsonSerializable<AlexaCard>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaCard);

    [JsonPropertyName("cardBackgroundColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? CardBackgroundColor { get; set; }

    [JsonPropertyName("cardId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? CardId { get; set; }

    [JsonPropertyName("cardRoundedCorner")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? CardRoundedCorner { get; set; }

    [JsonPropertyName("cardShadow")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? CardShadow { get; set; }

    [JsonPropertyName("emptyRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? EmptyRatingGraphic { get; set; }

    [JsonPropertyName("fullRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? FullRatingGraphic { get; set; }

    [JsonPropertyName("halfRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HalfRatingGraphic { get; set; }

    [JsonPropertyName("headerText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HeaderText { get; set; }

    [JsonPropertyName("imageAltText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ImageAltText { get; set; }

    [JsonPropertyName("imageCaption")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageCaption { get; set; }

    [JsonPropertyName("imageHideScrim")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageHideScrim { get; set; }

    [JsonPropertyName("imageProgressBarPercentage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? ImageProgressBarPercentage { get; set; }

    [JsonPropertyName("imageShowProgressBar")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageShowProgressBar { get; set; }

    [JsonPropertyName("imageSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ImageSource { get; set; }

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? PrimaryAction { get; set; }

    [JsonPropertyName("primaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? PrimaryText { get; set; }

    [JsonPropertyName("ratingGraphicType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<RatingGraphicType?>? RatingGraphicType { get; set; }

    [JsonPropertyName("ratingNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? RatingNumber { get; set; }

    [JsonPropertyName("ratingSlotMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<RatingSlotMode?>? RatingSlotMode { get; set; }

    [JsonPropertyName("ratingText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? RatingText { get; set; }

    [JsonPropertyName("secondaryIconName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryIconName { get; set; }

    [JsonPropertyName("secondaryIconSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryIconSource { get; set; }

    [JsonPropertyName("secondaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryText { get; set; }

    [JsonPropertyName("singleRatingGraphicWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? SingleRatingGraphicWidth { get; set; }

    [JsonPropertyName("singleRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SingleRatingGraphic { get; set; }

    [JsonPropertyName("tertiaryIconName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? TertiaryIconName { get; set; }

    [JsonPropertyName("tertiaryIconSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? TertiaryIconSource { get; set; }

    [JsonPropertyName("tertiaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? TertiaryText { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaCard
    {
        ResponsiveTemplate.RegisterTypeInfo<T>();
    }
}