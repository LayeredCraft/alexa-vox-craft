using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

public class SendTokenListDataDirective : ListDataDirective, IJsonSerializable<SendTokenListDataDirective>
{
    public const string DirectiveType = "Alexa.Presentation.APL.SendTokenListData";

    public static void AddSupport()
    {
        DirectiveConverter.RegisterDirectiveDerivedType<SendTokenListDataDirective>(DirectiveType);
    }

    [JsonPropertyName("type")]
    public override string Type => DirectiveType;

    [JsonPropertyName("pageToken")] public string PageToken { get; set; }

    [JsonPropertyName("nextPageToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? NextPageToken { get; set; }

    public new static void RegisterTypeInfo<T>() where T : SendTokenListDataDirective
    {
        ListDataDirective.RegisterTypeInfo<T>();
    }
}