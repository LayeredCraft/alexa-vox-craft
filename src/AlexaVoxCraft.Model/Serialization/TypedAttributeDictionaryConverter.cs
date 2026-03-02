using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Serialization;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="Dictionary{TKey,TValue}">Dictionary&lt;string, object?&gt;</see> that
/// persists type information alongside each value using a <c>_type</c> discriminator.
/// <para>
/// On <b>write</b>, each non-null value is wrapped as <c>{ "_type": "...", "_value": &lt;json&gt; }</c>.
/// Non-materialized enumerables (e.g., LINQ iterators) are <b>not allowed</b>; callers must store a concrete
/// collection type (e.g., <see cref="List{T}"/> or an array) by calling <c>.ToList()</c> / <c>.ToArray()</c>.
/// </para>
/// <para>
/// On <b>read</b>, the discriminator drives deserialization back to the original CLR type.
/// Values that lack <c>_type</c>/<c>_value</c> (legacy or Alexa-set) fall back to <see cref="object"/> deserialization,
/// which will use the registered <see cref="ObjectConverter"/>. No raw <see cref="JsonElement"/> leaks.
/// Unresolvable types also fall back to <see cref="object"/> via the same path.
/// </para>
/// </summary>
public sealed class TypedAttributeDictionaryConverter : JsonConverter<Dictionary<string, object?>>
{
    private const string TypeDiscriminator = "_type";
    private const string ValueProperty = "_value";

    public override Dictionary<string, object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject for attribute dictionary.");
        }

        var result = new Dictionary<string, object?>(StringComparer.Ordinal);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return result;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected PropertyName for attribute key.");
            }

            var key = reader.GetString() ?? throw new JsonException("Null attribute key.");
            reader.Read();

            result[key] = ReadValue(ref reader, options);
        }

        throw new JsonException("Unexpected end while reading attribute dictionary.");
    }

    private static object? ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        // Non-object value — legacy primitive/array written without wrapper.
        // Deserialize as object; this will use the registered ObjectConverter.
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            return JsonSerializer.Deserialize<object?>(ref reader, options);
        }

        // Buffer object so we can inspect for _type/_value regardless of property order.
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Wrapper shape: { "_type": "...", "_value": ... }
        if (root.TryGetProperty(TypeDiscriminator, out var typeProp) &&
            root.TryGetProperty(ValueProperty, out var valueProp))
        {
            var targetType = ResolveType(typeProp.GetString());

            if (targetType is not null)
            {
                return valueProp.Deserialize(targetType, options);
            }

            // Type present but not resolvable => fall back to object materialization (no JsonElement).
            return DeserializeElementAsObject(valueProp, options);
        }

        // Plain object (legacy or Alexa-set) => fall back to object materialization (no JsonElement).
        return DeserializeElementAsObject(root, options);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, object?> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var (key, val) in value)
        {
            writer.WritePropertyName(key);

            if (val is null)
            {
                writer.WriteNullValue();
                continue;
            }

            if (val is JsonElement jsonElement)
            {
                jsonElement.WriteTo(writer);
                continue;
            }

            // ✅ Enforce materialized collections (Option 1): throw if the runtime type is not safely rehydratable.
            EnsureMaterializedIfEnumerable(key, val);

            var runtimeType = val.GetType();

            writer.WriteStartObject();
            writer.WriteString(TypeDiscriminator, GetTypeDiscriminator(runtimeType));
            writer.WritePropertyName(ValueProperty);
            JsonSerializer.Serialize(writer, val, runtimeType, options);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }

    private static void EnsureMaterializedIfEnumerable(string key, object val)
    {
        // string is IEnumerable<char>, but it's a scalar for our purposes.
        if (val is string)
        {
            return;
        }

        if (val is IEnumerable)
        {
            var runtimeType = val.GetType();

            if (IsNonMaterializedEnumerable(runtimeType))
            {
                throw new JsonException(
                    $"Session attribute '{key}' has a non-materialized enumerable value of type '{runtimeType.FullName}'. " +
                    "Store a concrete collection type instead (e.g., call .ToList() or .ToArray() before assigning).");
            }
        }
    }

    private static bool IsNonMaterializedEnumerable(Type runtimeType)
    {
        // Arrays are fine
        if (runtimeType.IsArray)
        {
            return false;
        }

        var fullName = runtimeType.FullName;

        // Most common offender: LINQ iterator types (e.g., System.Linq.Enumerable+ArraySelectIterator...)
        if (fullName is not null &&
            fullName.StartsWith("System.Linq.Enumerable+", StringComparison.Ordinal))
        {
            return true;
        }

        // Interface/abstract enumerable types are not rehydratable as a concrete instance
        if (runtimeType.IsAbstract || runtimeType.IsInterface)
        {
            return true;
        }

        // Many iterator/enumerator implementations have no public parameterless ctor
        if (runtimeType.IsClass && runtimeType.GetConstructor(Type.EmptyTypes) is null)
        {
            return true;
        }

        return false;
    }

    private static Type? ResolveType(string? typeName) =>
        string.IsNullOrEmpty(typeName)
            ? null
            : Type.GetType(typeName, throwOnError: false, ignoreCase: false);

    private static object? DeserializeElementAsObject(JsonElement element, JsonSerializerOptions options)
    {
        // Rehydrate the JsonElement as a reader and deserialize as object.
        // This uses the registered ObjectConverter (JsonConverter<object>).
        var utf8 = Encoding.UTF8.GetBytes(element.GetRawText());
        var r = new Utf8JsonReader(utf8);
        r.Read();
        return JsonSerializer.Deserialize<object?>(ref r, options);
    }

    private static string GetTypeDiscriminator(Type type)
    {
        // Default: AssemblyQualifiedName is the most reliable for Type.GetType(...)
        return type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
    }
}