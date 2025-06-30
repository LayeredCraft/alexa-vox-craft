using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Response.Directive;

public class DialogUpdateDynamicEntities : IDirective
{
    public const string DirectiveType = "Dialog.UpdateDynamicEntities";

    [JsonPropertyName("type")]
    public string Type => DirectiveType;

    [JsonPropertyName("updateBehavior"), JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<UpdateBehavior>))]
    public UpdateBehavior UpdateBehavior { get; set; }

    [JsonPropertyName("types")]
    public List<SlotType> Types { get; set; } = [];
}