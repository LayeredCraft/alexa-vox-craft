using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaTextList : APLComponent, IJsonSerializable<AlexaTextList>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaTextList);


    [JsonPropertyName("backgroundAlign")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? BackgroundAlign { get; set; }

    [JsonPropertyName("backgroundBlur")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? BackgroundBlur { get; set; }

    [JsonPropertyName("backgroundColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? BackgroundColor { get; set; }

    [JsonPropertyName("backgroundImageSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? BackgroundImageSource { get; set; }

    [JsonPropertyName("backgroundVideoSource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<VideoSource>? BackgroundVideoSource { get; set; }

    [JsonPropertyName("backgroundScale")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Scale>? BackgroundScale { get; set; }

    [JsonPropertyName("colorOverlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ColorOverlay { get; set; }


    [JsonPropertyName("overlayGradient")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? OverlayGradient { get; set; }


    [JsonPropertyName("videoAudioTrack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? VideoAudioTrack { get; set; } = "foreground";


    [JsonPropertyName("videoAutoPlay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? VideoAutoPlay { get; set; }

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

    [JsonPropertyName("headerDivider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HeaderDivider { get; set; }

    [JsonPropertyName("hideDivider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HideDivider { get; set; }

    [JsonPropertyName("hideOrdinal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? HideOrdinal { get; set; }

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? PrimaryAction { get; set; }

    [JsonPropertyName("listItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<AlexaTextListItem>? ListItems { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("onSwipeDone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnSwipeDone { get; set; }

    [JsonPropertyName("onSwipeMove")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnSwipeMove { get; set; }

    [JsonPropertyName("swipeActionIconBackground")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SwipeActionIconBackground { get; set; }

    [JsonPropertyName("swipeActionIconForeground")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SwipeActionIconForeground { get; set; }

    [JsonPropertyName("optionsButton1Command")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OptionsButton1Command { get; set; }

    [JsonPropertyName("optionsButton1Text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? OptionsButton1Text { get; set; }

    [JsonPropertyName("optionsButton2Command")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OptionsButton2Command { get; set; }

    [JsonPropertyName("optionsButton2Text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? OptionsButton2Text { get; set; }

    [JsonPropertyName("swipeDirection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SwipeDirection { get; set; }

    [JsonPropertyName("swipeActionIcon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SwipeActionIcon { get; set; }

    [JsonPropertyName("swipeActionIconType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SwipeActionIconType { get; set; }

    [JsonPropertyName("lang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Lang { get; set; }

    [JsonPropertyName("headerAttributionOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? HeaderAttributionOpacity { get; set; }

    [JsonPropertyName("listId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ListId { get; set; }

    [JsonPropertyName("speechItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SpeechItems { get; set; }

    [JsonPropertyName("enableReorder")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? EnableReorder { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaTextList
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var headerBackButtonCommandProp = info.Properties.FirstOrDefault(p => p.Name == "headerBackButtonCommand");
            headerBackButtonCommandProp?.CustomConverter = new APLCommandListConverter(false);

            var primaryActionProp = info.Properties.FirstOrDefault(p => p.Name == "primaryAction");
            primaryActionProp?.CustomConverter = new APLCommandListConverter(false);

            var onSwipeDoneProp = info.Properties.FirstOrDefault(p => p.Name == "onSwipeDone");
            onSwipeDoneProp?.CustomConverter = new APLCommandListConverter(false);

            var onSwipeMoveProp = info.Properties.FirstOrDefault(p => p.Name == "onSwipeMove");
            onSwipeMoveProp?.CustomConverter = new APLCommandListConverter(false);

            var optionsButton1CommandProp = info.Properties.FirstOrDefault(p => p.Name == "optionsButton1Command");
            optionsButton1CommandProp?.CustomConverter = new APLCommandListConverter(false);

            var optionsButton2CommandProp = info.Properties.FirstOrDefault(p => p.Name == "optionsButton2Command");
            optionsButton2CommandProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}