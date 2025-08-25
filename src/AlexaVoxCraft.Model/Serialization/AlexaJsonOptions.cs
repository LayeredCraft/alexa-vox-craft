using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AlexaVoxCraft.Model.Serialization;

public static class AlexaJsonOptions
{
    // Thread-safe immutable collections for modifiers and converters (using ImmutableArray for better enumeration performance)
    private static ImmutableArray<Action<JsonTypeInfo>> _modifiers = ImmutableArray<Action<JsonTypeInfo>>.Empty;
    private static ImmutableArray<JsonConverter> _converters = ImmutableArray<JsonConverter>.Empty;

    // Cache invalidation using version counter instead of Lazy replacement
    private static volatile int _version = 0;
    private static volatile int _cachedVersion = -1;
    private static JsonSerializerOptions? _cachedOptions;
    private static readonly object _lock = new();

    public static JsonSerializerOptions DefaultOptions
    {
        get
        {
            var cachedOptions = _cachedOptions;
            var currentVersion = _version;
            
            // Check if we have cached options for current version
            if (cachedOptions is not null && _cachedVersion == currentVersion)
            {
                return cachedOptions;
            }

            lock (_lock)
            {
                // Double-check pattern with version validation
                cachedOptions = _cachedOptions;
                if (cachedOptions is not null && _cachedVersion == currentVersion)
                {
                    return cachedOptions;
                }

                var options = CreateOptions();
                _cachedOptions = options;
                _cachedVersion = currentVersion;
                return options;
            }
        }
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var resolver = new AlexaTypeResolver();
        resolver.Modifiers.Add(Modifiers.SetNumberHandlingModifier);

        // Add all registered modifiers thread-safely
        foreach (var modifier in _modifiers)
        {
            resolver.Modifiers.Add(modifier);
        }

        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = resolver,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        // Add all converters after JSON options are configured
        options.Converters.Add(new ObjectConverter());
        
        // Add all registered converters thread-safely  
        foreach (var converter in _converters)
        {
            options.Converters.Add(converter);
        }

        return options;
    }
    
    public static void RegisterConverter<T>(JsonConverter<T> converter) where T : notnull
    {
        lock (_lock)
        {
            // Update converters collection
            _converters = _converters.Add(converter);
            
            // Invalidate cache by incrementing version
            _version++;
            _cachedOptions = null;
        }
    }

    public static void RegisterTypeModifier<T>(Action<JsonTypeInfo> modifier)
    {
        // Create wrapped modifier for type matching
        var wrappedModifier = new Action<JsonTypeInfo>(ti =>
        {
            if (ti.Type == typeof(T))
            {
                modifier(ti);
            }
        });
        
        lock (_lock)
        {
            // Update modifiers collection
            _modifiers = _modifiers.Add(wrappedModifier);
            
            // Invalidate cache by incrementing version
            _version++;
            _cachedOptions = null;
        }
    }
}