using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Response;

[JsonConverter(typeof(DirectiveConverter))]
public interface IDirective
{
    string Type { get; }
}