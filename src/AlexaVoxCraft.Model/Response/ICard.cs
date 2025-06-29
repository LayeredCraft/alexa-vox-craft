using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Response;

[JsonConverter(typeof(CardConverter))]
public interface ICard : IResponse
{
}