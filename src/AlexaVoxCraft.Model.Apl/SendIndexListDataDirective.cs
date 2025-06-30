using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

public class SendIndexListDataDirective : ListDataDirective, IJsonSerializable<SendIndexListDataDirective>
{
    public const string DirectiveType = "Alexa.Presentation.APL.SendIndexListData";

    public static void AddSupport()
    {
        DirectiveConverter.RegisterDirectiveDerivedType<SendIndexListDataDirective>(DirectiveType);
    }

    [JsonPropertyName("type")] public override string Type => DirectiveType;

    [JsonPropertyName("listVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ListVersion { get; set; }

    [JsonPropertyName("startIndex")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StartIndex { get; set; }

    [JsonPropertyName("minimumInclusiveIndex")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinimumInclusiveIndex { get; set; }

    [JsonPropertyName("maximumExclusiveIndex")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaximumExclusiveIndex { get; set; }

    public new static void RegisterTypeInfo<T>() where T : SendIndexListDataDirective
    {
        ListDataDirective.RegisterTypeInfo<T>();
    }
}