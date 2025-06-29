using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<AlexaImageAspectRatio>))]
public enum AlexaImageAspectRatio
{
    [EnumMember(Value="square")]
    Square,
    [EnumMember(Value = "round")]
    Round,
    [EnumMember(Value = "standard_landscape")]
    StandardLandscape,
    [EnumMember(Value = "standard_portrait")]
    StandardPortrait,
    [EnumMember(Value = "poster_landscape")]
    PosterLandscape,
    [EnumMember(Value = "poster_portrait")]
    PosterPortrait,
    [EnumMember(Value = "widescreen")]
    Widescreen
}