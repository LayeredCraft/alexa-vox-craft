using System.Text.Json;
using AlexaVoxCraft.Model.ConnectionTasks;
using AlexaVoxCraft.Model.ConnectionTasks.Inputs;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Response.Converters;

public class ConnectionTaskConverter : BasePolymorphicConverter<IConnectionTask>
{
    private static readonly List<IConnectionTaskResolver> ConnectionTaskResolvers = [new PinConfirmationResolver()];

    public static void AddToConnectionTaskResolvers(IConnectionTaskResolver resolver)
    {
        if (ConnectionTaskResolvers.All(rc => rc.GetType() != resolver.GetType()))
        {
            ConnectionTaskResolvers.Add(resolver);
        }
    }

    protected override Func<JsonElement, string?> KeyResolver => element =>
    {
        // Look for @type and @version
        var typeName = element.TryGetProperty(TypeDiscriminatorPropertyName, out var typeProp)
            ? typeProp.GetString()
            : null;

        var versionName = element.TryGetProperty("@version", out var versionProp)
            ? versionProp.GetString()
            : null;

        if (string.IsNullOrWhiteSpace(typeName) || string.IsNullOrWhiteSpace(versionName))
            return null;
        
        return $"{typeName}/{versionName}";
    };

    protected override Func<JsonElement, Type?> CustomTypeResolver =>
        element =>
        {
            // Check task resolvers
            var converter = ConnectionTaskResolvers.FirstOrDefault(c => c.CanResolve(element));
            return converter?.Resolve(element);
        };

    protected override string TypeDiscriminatorPropertyName => "@type";

    protected override IDictionary<string, Type> DerivedTypes => new Dictionary<string, Type>
    {
        { PrintPdfV1.ConnectionKey, typeof(PrintPdfV1) },
        { PrintImageV1.ConnectionKey, typeof(PrintImageV1) },
        { PrintWebPageV1.ConnectionKey, typeof(PrintWebPageV1) },
        { ScheduleTaxiReservation.ConnectionKey, typeof(ScheduleTaxiReservation) },
        { ScheduleFoodEstablishmentReservation.ConnectionKey, typeof(ScheduleFoodEstablishmentReservation) }
    };

    protected override IDictionary<string, Func<JsonElement, Type>> DataDrivenTypeFactories =>
        new Dictionary<string, Func<JsonElement, Type>>();

    protected override Type? DefaultType => null;
}