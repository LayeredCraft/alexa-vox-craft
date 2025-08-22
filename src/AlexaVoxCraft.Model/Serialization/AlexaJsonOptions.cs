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

    // Cached options for performance - invalidated when modifiers/converters are added
    private static JsonSerializerOptions? _cachedOptions;
    private static readonly object _lock = new();

    public static JsonSerializerOptions DefaultOptions
    {
        get
        {
            if (_cachedOptions is not null)
            {
                return _cachedOptions;
            }

            lock (_lock)
            {
                if (_cachedOptions is not null)
                {
                    return _cachedOptions;
                }

                var resolver = new AlexaTypeResolver();
                resolver.Modifiers.Add(Modifiers.SetNumberHandlingModifier);

                foreach (var modifier in AdditionalModifiers)
                {
                    resolver.Modifiers.Add(modifier);
                }

                // Combine all resolvers: source generation contexts first, then custom resolver
                var allResolvers = new List<IJsonTypeInfoResolver> { AlexaJsonContext.Default };
                allResolvers.AddRange(AdditionalResolvers);
                allResolvers.Add(resolver);
                
                var options = new JsonSerializerOptions
                {
                    TypeInfoResolver = JsonTypeInfoResolver.Combine([.. allResolvers]),
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                
                options.Converters.Add(new ObjectConverter());
                
                foreach (var converter in AdditionalConverters)
                {
                    options.Converters.Add(converter);
                }
                
                _cachedOptions = options;
                return options;
            }
        }
    }
    
    public static void RegisterConverter<T>(JsonConverter<T> converter) where T : notnull
    {
        lock (_lock)
        {
            AdditionalConverters.Add(converter);
            _cachedOptions = null; // Invalidate cache
        }
    }

    public static void RegisterTypeModifier<T>(Action<JsonTypeInfo> modifier)
    {
        lock (_lock)
        {
            AdditionalModifiers.Add(ti => {
                if (ti.Type == typeof(T))
                {
                    modifier(ti);
                }
            });
            _cachedOptions = null; // Invalidate cache
        }
    }
    
    public static void RegisterTypeInfoResolver(IJsonTypeInfoResolver resolver)
    {
        lock (_lock)
        {
            AdditionalResolvers.Add(resolver);
            _cachedOptions = null; // Invalidate cache
        }
    }
}