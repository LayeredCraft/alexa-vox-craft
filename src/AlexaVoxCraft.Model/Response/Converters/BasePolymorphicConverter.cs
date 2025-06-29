using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Converters;

public abstract class BasePolymorphicConverter<T> : JsonConverter<T>
{
    protected virtual string TypeDiscriminatorPropertyName => "type";
    protected abstract IDictionary<string, Type> DerivedTypes { get; }

    protected virtual IDictionary<string, Func<JsonElement, Type>> DataDrivenTypeFactories =>
        new Dictionary<string, Func<JsonElement, Type>>();

    protected virtual Func<JsonElement, string?> KeyResolver => element =>
    {
        var typeValue = element.TryGetProperty(TypeDiscriminatorPropertyName, out var typeProp)
            ? typeProp.GetString()
            : null;
        return typeValue;
    };

    protected virtual Func<JsonElement, Type?>? CustomTypeResolver => null;

    protected virtual JsonElement TransformJson(JsonElement original)
    {
        return original; // No-op by default
    }

    protected virtual Type? DefaultType => null;

    protected virtual bool EmptyObjectIsNull => false;

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        Type? resultType = null;
        var typeValue = KeyResolver(root);

        if (string.IsNullOrWhiteSpace(typeValue) && CustomTypeResolver is not null)
        {
            resultType = CustomTypeResolver(root);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(typeValue))
                throw new JsonException("Missing 'type' field");

            resultType = DefaultType;

            if (DerivedTypes.TryGetValue(typeValue, out var type))
            {
                resultType = type;
            }
            else if (DataDrivenTypeFactories.TryGetValue(typeValue, out var dataFactory))
            {
                resultType = dataFactory(root);
            }
        }

        if (resultType is null)
        {
            throw new JsonException($"Unrecognized type '{typeValue}'");
        }

        // Allow subclasses to mutate the element
        var transformed = TransformJson(root);
        
        return (T)JsonSerializer.Deserialize(transformed.GetRawText(), resultType, options)!;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
    
    private bool IsEmptyObject(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.Object && !element.EnumerateObject().Any();
    }
}