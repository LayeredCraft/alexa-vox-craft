using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Package;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<TargetViewport>))]
public enum TargetViewport
{
    [EnumMember(Value="WIDGET_M")]
    WidgetM
}