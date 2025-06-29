using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AlexaVoxCraft.Model.Serialization;

public static class AlexaJsonOptions
{
    // This allows external packages to register their own JsonTypeInfo modifiers
    private static readonly List<Action<JsonTypeInfo>> AdditionalModifiers = [];
    
    private static readonly List<JsonConverter> AdditionalConverters = [];

    public static JsonSerializerOptions DefaultOptions
    {
        get
        {
            var resolver = new AlexaTypeResolver();

            resolver.Modifiers.Add(Modifiers.SetNumberHandlingModifier);

            foreach (var modifier in AdditionalModifiers.ToList())
            {
                resolver.Modifiers.Add(modifier);
            }

            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = resolver
            };
            
            options.Converters.Add(new ObjectConverter());
            
            foreach (var converter in AdditionalConverters.ToList())
            {
                options.Converters.Add(converter);
            }
            
            options.ReadCommentHandling = JsonCommentHandling.Skip;
            
            return options;
        }
    }
    
    public static void RegisterConverter<T>(JsonConverter<T> converter) where T : notnull
    {
        AdditionalConverters.Add(converter);
    }

    public static void RegisterTypeModifier<T>(Action<JsonTypeInfo> modifier)
    {
        AdditionalModifiers.Add(ti => {
            if (ti.Type == typeof(T))
            {
                modifier(ti);
            }
        });
    }
}