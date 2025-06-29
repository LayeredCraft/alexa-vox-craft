using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaSwipeToAction : APLComponent, IJsonSerializable<AlexaSwipeToAction>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaSwipeToAction);

    [JsonPropertyName("actionIcon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ActionIcon { get; set; }

    [JsonPropertyName("actionIconBackground")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ActionIconBackground { get; set; }

    [JsonPropertyName("actionIconForeground")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ActionIconForeground { get; set; }

    [JsonPropertyName("actionIconType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ActionIconType { get; set; }

    [JsonPropertyName("button1Command")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? Button1Command { get; set; }

    [JsonPropertyName("button1Text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Button1Text { get; set; }

    [JsonPropertyName("button2Command")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? Button2Command { get; set; }

    [JsonPropertyName("button2Text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Button2Text { get; set; }

    [JsonPropertyName("buttonsSpacingRight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? ButtonsSpacingRight { get; set; }

    [JsonPropertyName("buttonsSpacingTop")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? ButtonsSpacingTop { get; set; }

    [JsonPropertyName("customLayoutName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? CustomLayoutName { get; set; }

    [JsonPropertyName("direction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Direction { get; set; }

    [JsonPropertyName("emptyRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? EmptyRatingGraphic { get; set; }

    [JsonPropertyName("fullRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? FullRatingGraphic { get; set; }

    [JsonPropertyName("halfRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HalfRatingGraphic { get; set; }

    [JsonPropertyName("hideDivider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HideDivider { get; set; }

    [JsonPropertyName("hideOrdinal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HideOrdinal { get; set; }

    [JsonPropertyName("hideHorizontalMargin")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HideHorizontalMargin { get; set; }

    [JsonPropertyName("imageAlignment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<AlexaImageAlignment?>? ImageAlignment { get; set; }

    [JsonPropertyName("imageBlurredBackground")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ImageBlurredBackground { get; set; }

    [JsonPropertyName("imageScale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Scale?>? Scale { get; set; }

    [JsonPropertyName("imageThumbnailSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ImageThumbnailSource { get; set; }

    [JsonPropertyName("onButtonsHidden")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnButtonsHidden { get; set; }

    [JsonPropertyName("onButtonsShown")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnButtonsShown { get; set; }

    [JsonPropertyName("onSwipeDone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnSwipeDone { get; set; }

    [JsonPropertyName("onSwipeMove")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnSwipeMove { get; set; }

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? PrimaryAction { get; set; }

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

    [JsonPropertyName("secondaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryText { get; set; }

    [JsonPropertyName("secondaryTextPosition")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryTextPosition { get; set; }

    [JsonPropertyName("singleRatingGraphicWidth")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? SingleRatingGraphicWidth { get; set; }

    [JsonPropertyName("singleRatingGraphic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SingleRatingGraphic { get; set; }

    [JsonPropertyName("tertiaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? TertiaryText { get; set; }

    [JsonPropertyName("tertiaryTextPosition")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? TertiaryTextPosition { get; set; }

    [JsonPropertyName("touchForward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? TouchForward { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("lang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Lang { get; set; }

    [JsonPropertyName("componentSlot")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Component>? ComponentSlot { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaSwipeToAction
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var button1CommandProp = info.Properties.FirstOrDefault(p => p.Name == "button1Command");
            if (button1CommandProp is not null)
            {
                button1CommandProp.CustomConverter = new APLCommandListConverter(false);
            }

            var button2CommandProp = info.Properties.FirstOrDefault(p => p.Name == "button2Command");
            if (button2CommandProp is not null)
            {
                button2CommandProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onButtonsHiddenProp = info.Properties.FirstOrDefault(p => p.Name == "onButtonsHidden");
            if (onButtonsHiddenProp is not null)
            {
                onButtonsHiddenProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onButtonsShownProp = info.Properties.FirstOrDefault(p => p.Name == "onButtonsShown");
            if (onButtonsShownProp is not null)
            {
                onButtonsShownProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onSwipeDoneProp = info.Properties.FirstOrDefault(p => p.Name == "onSwipeDone");
            if (onSwipeDoneProp is not null)
            {
                onSwipeDoneProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onSwipeMoveProp = info.Properties.FirstOrDefault(p => p.Name == "onSwipeMove");
            if (onSwipeMoveProp is not null)
            {
                onSwipeMoveProp.CustomConverter = new APLCommandListConverter(false);
            }

            var primaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "primaryAction");
            if (primaryActionProp is not null)
            {
                primaryActionProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}