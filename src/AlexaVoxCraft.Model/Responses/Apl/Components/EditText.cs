using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Responses.Apl.Commands;

namespace AlexaVoxCraft.Model.Responses.Apl.Components;

public class EditText : ActionableComponent
{
    [JsonPropertyName("borderColor"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> BorderColor { get; set; }

    [JsonPropertyName("borderStrokeWidth"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLAbsoluteDimensionValue BorderStrokeWidth { get; set; }

    [JsonPropertyName("borderWidth"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLAbsoluteDimensionValue BorderWidth { get; set; }

    [JsonPropertyName("color"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Color { get; set; }

    [JsonPropertyName("fontFamily"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FontFamily { get; set; }

    [JsonPropertyName("fontSize"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> FontSize { get; set; }

    [JsonPropertyName("fontStyle"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FontStyle { get; set; }

    [JsonPropertyName("fontWeight"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> FontWeight { get; set; }

    [JsonPropertyName("highlightColor"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HighlightColor { get; set; }

    [JsonPropertyName("hint"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Hint { get; set; }

    [JsonPropertyName("hintColor"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HintColor { get; set; }

    [JsonPropertyName("hintStyle"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HintStyle { get; set; }

    [JsonPropertyName("hintWeight"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HintWeight { get; set; }

    [JsonPropertyName("keyboardType"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<KeyboardType?> KeyboardType { get; set; }

    [JsonPropertyName("maxLength"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int> MaxLength { get; set; }

    [JsonPropertyName("onTextChange"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IEnumerable<APLCommand>> OnTextChange { get; set; }

    [JsonPropertyName("onSubmit"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IEnumerable<APLCommand>> OnSubmit { get; set; }

    [JsonPropertyName("secureInput"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> SecureInput { get; set; }

    [JsonPropertyName("selectOnFocus"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> SelectOnFocus { get; set; }

    [JsonPropertyName("size"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> Size { get; set; }

    [JsonPropertyName("submitKeyType"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<SubmitKeyType?> SubmitKeyType { get; set; }

    [JsonPropertyName("text"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Text { get; set; }

    [JsonPropertyName("validCharacters"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ValidCharacters { get; set; }

    [JsonPropertyName("lang"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Lang { get; set; }
}