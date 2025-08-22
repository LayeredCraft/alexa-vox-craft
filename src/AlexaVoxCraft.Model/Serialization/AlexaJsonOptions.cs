using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AlexaVoxCraft.Model.Serialization;

public static class AlexaJsonOptions
{
    // This allows external packages to register their own JsonTypeInfo modifiers
    private static readonly List<Action<JsonTypeInfo>> AdditionalModifiers = [];
    
    private static readonly List<JsonConverter> AdditionalConverters = [];
    
    private static readonly List<IJsonTypeInfoResolver> AdditionalResolvers = [];

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

            // Combine all resolvers: source generation contexts first, then custom resolver
            var allResolvers = new List<IJsonTypeInfoResolver> { AlexaJsonContext.Default };
            allResolvers.AddRange(AdditionalResolvers);
            allResolvers.Add(resolver);
            
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = JsonTypeInfoResolver.Combine(allResolvers.ToArray())
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
    
    public static void RegisterTypeInfoResolver(IJsonTypeInfoResolver resolver)
    {
        AdditionalResolvers.Add(resolver);
    }
}