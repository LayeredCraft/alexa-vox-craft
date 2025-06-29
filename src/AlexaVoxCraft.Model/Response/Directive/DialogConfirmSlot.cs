using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.Response.Directive;

public class DialogConfirmSlot : IDirective
{
    public const string DirectiveType = "Dialog.ConfirmSlot";

    [JsonPropertyName("type")]
    public string Type => DirectiveType;

    [JsonPropertyName("slotToConfirm"), JsonRequired]
    public string SlotName { get; set; }

    [JsonPropertyName("updatedIntent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Intent? UpdatedIntent { get; set; }

    public DialogConfirmSlot(string slotName)
    {
        SlotName = slotName;
    }

    internal DialogConfirmSlot()
    {
    }
}