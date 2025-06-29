using System;
using System.Collections.Generic;
using System.Text.Json;
using AlexaVoxCraft.Model.Apl.DataStore;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class DataStoreCommandConverter : BasePolymorphicConverter<DataStoreCommand>
{
    private static readonly Dictionary<string, Type> DataStoreCommandLookup = new()
    {
        { PutNamespace.CommandType, typeof(PutNamespace) },
        { RemoveNamespace.CommandType, typeof(RemoveNamespace) },
        { Clear.CommandType, typeof(Clear) }
    };

    protected override IDictionary<string, Type> DerivedTypes => DataStoreCommandLookup;

    protected override IDictionary<string, Func<JsonElement, Type>> DataDrivenTypeFactories =>
        new Dictionary<string, Func<JsonElement, Type>>
        {
            [PutObject.CommandType] = element =>
            {
                if (element.TryGetProperty("content", out var contentElement) &&
                    contentElement.ValueKind == JsonValueKind.Array)
                {
                    return typeof(PutObjectArray);
                }

                return typeof(PutObject);
            },
        };
}