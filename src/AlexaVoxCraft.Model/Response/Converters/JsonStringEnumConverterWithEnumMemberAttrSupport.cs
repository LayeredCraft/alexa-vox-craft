using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Helpers;

namespace AlexaVoxCraft.Model.Response.Converters;
//
// public sealed class JsonStringEnumConverterWithEnumMemberAttrSupport<
//     [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum> : JsonStringEnumConverter<TEnum>
//     where TEnum : struct, Enum
// {
//     public JsonStringEnumConverterWithEnumMemberAttrSupport() : base(namingPolicy: ResolveNamingPolicy())
//     {
//     }
//
//     private static JsonNamingPolicy? ResolveNamingPolicy()
//     {
//         var map = typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static)
//             .Select(f => (f.Name, AttributeName: f.GetCustomAttribute<EnumMemberAttribute>()?.Value))
//             .Where(pair => pair.AttributeName != null)
//             .ToDictionary();
//
//         return map.Count > 0 ? new EnumMemberNamingPolicy(map!) : null;
//     }
//     
//     private sealed class EnumMemberNamingPolicy(Dictionary<string, string> map) : JsonNamingPolicy
//     {
//         public override string ConvertName(string name)
//             => map.TryGetValue(name, out string? newName) ? newName : name;
//     }
//     
//     public static string ToEnumString(TEnum value)
//     {
//         var name = Enum.GetName(typeof(TEnum), value);
//         var enumMember = typeof(TEnum).GetField(name!)
//             ?.GetCustomAttribute<EnumMemberAttribute>();
//         return enumMember?.Value ?? name!;
//     }
//     
//     
// }

public class JsonStringEnumConverterWithEnumMemberAttrSupport<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    public virtual TEnum? FallbackValue { get; set; }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (EnumHelper.TryParseEnumWithEnumMemberSupport<TEnum>(str, out var parsed))
        {
            return parsed;
        }

        if (FallbackValue.HasValue)
        {
            return FallbackValue.Value;
        }

        throw new JsonException($"Unrecognized value '{str}' for enum '{typeof(TEnum).Name}'");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(EnumHelper.ToEnumString(value));
    }
} 
