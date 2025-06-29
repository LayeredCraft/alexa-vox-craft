using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Intent
{
    private string _name;

    [JsonPropertyName("name")]
    public string Name {
        get { return _name; }
        set {
            _name = value;
            Signature = value;
        }
    }

    [JsonIgnore]
    public IntentSignature Signature { get; private set; }


    [JsonPropertyName("confirmationStatus")]
    public string ConfirmationStatus { get; set; }

    [JsonPropertyName("slots")]
    public Dictionary<string, Slot> Slots { get; set; }
}