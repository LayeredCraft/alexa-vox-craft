using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Request.Type;

public class SkillEventPersistenceStatus
{
    [JsonPropertyName("userInformationPersistenceStatus"),
     JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<PersistenceStatus>))]
    public PersistenceStatus Status { get; set; }
}