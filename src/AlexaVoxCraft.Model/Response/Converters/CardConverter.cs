using System.Text.Json;

namespace AlexaVoxCraft.Model.Response.Converters;

public class CardConverter : BasePolymorphicConverter<ICard>
{
    public static Dictionary<string, Type> CardDerivedTypes = new()
    {
        { SimpleCard.CardType, typeof(SimpleCard) },
        { StandardCard.CardType, typeof(StandardCard) },
        { LinkAccountCard.CardType, typeof(LinkAccountCard) },
        { AskForPermissionsConsentCard.CardType, typeof(AskForPermissionsConsentCard) }
    };
    
    protected override Func<JsonElement, string?> KeyResolver => element =>
    {
        static string? GetProp(JsonElement el, string name) =>
            el.TryGetProperty(name, out var prop) ? prop.GetString() : null;

        return GetProp(element, TypeDiscriminatorPropertyName) ?? GetProp(element, "Type");
    };

    protected override string TypeDiscriminatorPropertyName => "type";
    protected override IDictionary<string, Type> DerivedTypes => CardDerivedTypes;

    protected override IDictionary<string, Func<JsonElement, Type>> DataDrivenTypeFactories =>
        new Dictionary<string, Func<JsonElement, Type>>();

    protected override Func<JsonElement, Type?>? CustomTypeResolver => null;
    protected override Type? DefaultType => null;
}