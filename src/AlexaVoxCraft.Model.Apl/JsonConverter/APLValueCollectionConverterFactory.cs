using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLValueCollectionConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        return typeToConvert.GetGenericTypeDefinition() == typeof(APLValueCollection<>);
    }

    public override System.Text.Json.Serialization.JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];

        return (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(
            typeof(APLValueCollectionConverter<>).MakeGenericType(itemType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: [false],
            culture: null)!;
    }
}