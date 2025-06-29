using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public abstract class AlexaProgressBarBase : APLComponent, IJsonSerializable<AlexaProgressBarBase>
{
    [JsonPropertyName("bufferValue")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? BufferValue { get; set; }

    [JsonPropertyName("isLoading")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? IsLoading { get; set; }

    [JsonPropertyName("progressBarType")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ProgressBarType?>? ProgressBarType { get; set; }

    [JsonPropertyName("progressFillColor")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ProgressFillColor { get; set; }

    [JsonPropertyName("progressValue")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? ProgressValue { get; set; }

    [JsonPropertyName("theme")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("totalValue")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? TotalValue { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaProgressBarBase
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}