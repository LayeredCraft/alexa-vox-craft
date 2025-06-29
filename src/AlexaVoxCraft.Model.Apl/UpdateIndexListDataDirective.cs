using System.Collections.Generic;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

public class UpdateIndexListDataDirective : IDirective
{
    public const string DirectiveType = "Alexa.Presentation.APL.UpdateIndexListData";

    public static void AddSupport()
    {
        DirectiveConverter.RegisterDirectiveDerivedType<UpdateIndexListDataDirective>(DirectiveType);
    }

    [JsonPropertyName("type")] public string Type => DirectiveType;

    [JsonPropertyName("token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }

    [JsonPropertyName("listId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ListId { get; set; }

    [JsonPropertyName("listVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ListVersion { get; set; }

    [JsonPropertyName("operations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Operation.Operation>? Operations { get; set; } = [];
}