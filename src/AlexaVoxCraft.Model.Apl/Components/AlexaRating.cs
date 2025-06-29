using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaRating : APLComponent, IJsonSerializable<AlexaRating>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaRating);

    [JsonPropertyName("emptyRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? EmptyRatingGraphic { get; set; }

    [JsonPropertyName("fullRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? FullRatingGraphic { get; set; }

    [JsonPropertyName("halfRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HalfRatingGraphic { get; set; }

    [JsonPropertyName("ratingGraphicType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<RatingGraphicType?>? RatingGraphicType { get; set; }

    [JsonPropertyName("ratingNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? RatingNumber { get; set; }

    [JsonPropertyName("ratingSlotMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<RatingSlotMode?>? RatingSlotMode { get; set; }

    [JsonPropertyName("ratingSlotPadding")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? RatingSlotPadding { get; set; }

    [JsonPropertyName("ratingText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? RatingText { get; set; }

    [JsonPropertyName("singleRatingGraphicWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? SingleRatingGraphicWidth { get; set; }

    [JsonPropertyName("singleRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SingleRatingGraphic { get; set; }

    [JsonPropertyName("ratingTextOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? RatingTextOpacity { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaRating
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}