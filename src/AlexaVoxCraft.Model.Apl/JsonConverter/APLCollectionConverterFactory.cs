using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLCollectionConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        return typeToConvert.GetGenericTypeDefinition() == typeof(APLCollection<>);
    }

    public override System.Text.Json.Serialization.JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];

        return (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(
            typeof(APLCollectionConverter<>).MakeGenericType(itemType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: [false],
            culture: null)!;
    }
}