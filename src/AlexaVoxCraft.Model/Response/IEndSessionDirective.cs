using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response;

public interface IEndSessionDirective: IDirective
{
    [JsonIgnore]
    bool? ShouldEndSession { get; }
}