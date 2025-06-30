using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaDetail : ResponsiveTemplate, IJsonSerializable<AlexaDetail>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaDetail);

    [JsonPropertyName("bodyText")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? BodyText { get; set; }

    [JsonPropertyName("button1AccessibilityLabel")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Button1AccessibilityLabel { get; set; }

    [JsonPropertyName("button1PrimaryAction")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? Button1PrimaryAction { get; set; }

    [JsonPropertyName("button1Style")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Button1Style { get; set; }

    [JsonPropertyName("button1Text")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Button1Text { get; set; }

    [JsonPropertyName("button1Theme")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Button1Theme { get; set; }

    [JsonPropertyName("button2PrimaryAction")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? Button2PrimaryAction { get; set; }

    [JsonPropertyName("button2Style")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Button2Style { get; set; }

    [JsonPropertyName("button2Text")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Button2Text { get; set; }

    [JsonPropertyName("button2Theme")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Button2Theme { get; set; }

    [JsonPropertyName("detailImageAlignment")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? DetailImageAlignment { get; set; }

    [JsonPropertyName("detailType")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? DetailType { get; set; }

    [JsonPropertyName("emptyRatingGraphic")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? EmptyRatingGraphic { get; set; }

    [JsonPropertyName("fullRatingGraphic")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? FullRatingGraphic { get; set; }

    [JsonPropertyName("halfRatingGraphic")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HalfRatingGraphic { get; set; }

    [JsonPropertyName("ratingGraphicType")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<RatingGraphicType?>? RatingGraphicType { get; set; }

    [JsonPropertyName("ratingNumber")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? RatingNumber { get; set; }

    [JsonPropertyName("ratingSlotMode")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<RatingSlotMode?>? RatingSlotMode { get; set; }

    [JsonPropertyName("ratingSlotPadding")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? RatingSlotPadding { get; set; }

    [JsonPropertyName("ratingText")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? RatingText { get; set; }

    [JsonPropertyName("singleRatingGraphicWidth")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? SingleRatingGraphicWidth { get; set; }

    [JsonPropertyName("singleRatingGraphic")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SingleRatingGraphic { get; set; }

    [JsonPropertyName("imageAlignment")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AlexaImageAlignment?>? ImageAlignment { get; set; }

    [JsonPropertyName("imageAspectRatio")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AlexaImageAspectRatio?>? ImageAspectRatio { get; set; }

    [JsonPropertyName("imageBlurredBackground")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageBlurredBackground { get; set; }

    [JsonPropertyName("imageHeight")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? ImageHeight { get; set; }

    [JsonPropertyName("imageWidth")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? ImageWidth { get; set; }

    [JsonPropertyName("imageRoundedCorner")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageRoundedCorner { get; set; }

    [JsonPropertyName("imageScale")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Scale>? Scale { get; set; }

    [JsonPropertyName("imageSource")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ImageSource { get; set; }

    [JsonPropertyName("imageCaption")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ImageCaption { get; set; }

    [JsonPropertyName("ingredientsHideDivider")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? IngredientsHideDivider { get; set; }

    [JsonPropertyName("ingredientsText")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? IngredientsText { get; set; }

    [JsonPropertyName("locationText")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? LocationText { get; set; }

    [JsonPropertyName("primaryText")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? PrimaryText { get; set; }

    [JsonPropertyName("quaternaryText")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? QuaternaryText { get; set; }

    [JsonPropertyName("scrollViewId")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ScrollViewId { get; set; }

    [JsonPropertyName("secondaryText")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryText { get; set; }

    [JsonPropertyName("tertiaryText")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? TertiaryText { get; set; }

    [JsonPropertyName("ingredientListItems")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<IngredientListItem>>? IngredientListItems { get; set; }

    [JsonPropertyName("imageShadow")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageShadow { get; set; }

    [JsonPropertyName("lang")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Lang { get; set; }

    [JsonPropertyName("headerAttributionOpacity")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? HeaderAttributionOpacity { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaDetail
    {
        ResponsiveTemplate.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var button1PrimaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "button1PrimaryAction");
            if (button1PrimaryActionProp is not null)
            {
                button1PrimaryActionProp.CustomConverter = new APLCommandListConverter(false);
            }

            var button2PrimaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "button2PrimaryAction");
            if (button2PrimaryActionProp is not null)
            {
                button2PrimaryActionProp.CustomConverter = new APLCommandListConverter(false);
            }

            var ingredientListItemsProp = info.Properties.FirstOrDefault(p => p.Name == "ingredientListItems");
            if (ingredientListItemsProp is not null)
            {
                ingredientListItemsProp.CustomConverter = new GenericSingleOrListConverter<IngredientListItem>(false);
            }
        });
    }
}