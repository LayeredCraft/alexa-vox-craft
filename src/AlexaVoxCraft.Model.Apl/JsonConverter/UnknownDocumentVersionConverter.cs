using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class UnknownDocumentVersionConverter : JsonStringEnumConverterWithEnumMemberAttrSupport<APLDocumentVersion>
{
    public override APLDocumentVersion? FallbackValue => APLDocumentVersion.Unknown;
}