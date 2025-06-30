using System.Collections.Generic;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

public class RenderDocumentDirective : IDirective
{
    public const string APLDirectiveType = "Alexa.Presentation.APL.RenderDocument";
    public const string APLTDirectiveType = "Alexa.Presentation.APLT.RenderDocument";
    public const string APLADirectiveType = "Alexa.Presentation.APLA.RenderDocument";

    public RenderDocumentDirective(){}

    public RenderDocumentDirective(APLDocumentReference document)
    {
        Document = document;
    }

    public static void AddSupport()
    {
        DirectiveConverter.RegisterDirectiveDerivedType<RenderDocumentDirective>(APLDirectiveType);
        DirectiveConverter.RegisterDirectiveDerivedType<RenderDocumentDirective>(APLTDirectiveType);
        DirectiveConverter.RegisterDirectiveDerivedType<RenderDocumentDirective>(APLADirectiveType);
    }

    [JsonPropertyName("type")]
    public string Type => Document switch
    {
        APLDocument _ => APLDirectiveType,
        APLDocumentLink _ => APLDirectiveType,
        APLTDocument _ => APLTDirectiveType,
        APLADocument _ => APLADirectiveType,
        _ => string.Empty
    };

    [JsonPropertyName("token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Token { get; set; }

    [JsonPropertyName("targetProfile")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLTProfile? TargetProfile { get; set; }

    [JsonPropertyName("document")]
    public APLDocumentReference Document { get; set; }

    [JsonPropertyName("datasources")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, APLDataSource>? DataSources { get; set; }

    [JsonPropertyName("sources")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, APLDocumentReference>? Sources { get; set; }
}