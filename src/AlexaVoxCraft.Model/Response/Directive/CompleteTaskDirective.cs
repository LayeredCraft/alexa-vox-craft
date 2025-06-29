using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class CompleteTaskDirective:IDirective
{
    public const string DirectiveType = "Tasks.CompleteTask";
    public CompleteTaskDirective() { }

    public CompleteTaskDirective(int statusCode, string statusMessage)
    {
        Status = new ConnectionStatus(statusCode,statusMessage);
    }

    [JsonPropertyName("type")]
    public string Type => DirectiveType;

    [JsonPropertyName("status")]
    public ConnectionStatus Status { get; set; } 
}