using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.Response;

[JsonConverter(typeof(OutputSpeechConverter))]
public interface IOutputSpeech : IResponse
{
    PlayBehavior? PlayBehavior { get; set; }
}