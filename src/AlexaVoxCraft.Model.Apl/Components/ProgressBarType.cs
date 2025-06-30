using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ProgressBarType>))]
public enum ProgressBarType
{
    [EnumMember(Value="determinate")]
    Determinate,
    [EnumMember(Value="indeterminate")]
    Indeterminate
}