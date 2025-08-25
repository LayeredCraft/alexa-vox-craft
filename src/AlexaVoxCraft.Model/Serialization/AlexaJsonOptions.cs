using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AlexaVoxCraft.Model.Serialization;

public static class AlexaJsonOptions
{
    // Thread-safe immutable collections for modifiers and converters
    private static ImmutableList<Action<JsonTypeInfo>> _modifiers = ImmutableList<Action<JsonTypeInfo>>.Empty;
    private static ImmutableList<JsonConverter> _converters = ImmutableList<JsonConverter>.Empty;

    // Thread-safe lazy initialization for options
    private static Lazy<JsonSerializerOptions> _lazyOptions = new(CreateOptions);

    public static JsonSerializerOptions DefaultOptions => _lazyOptions.Value;

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
        // Atomically update the converters collection
        ImmutableInterlocked.Update(ref _converters, list => list.Add(converter));
        
        // Invalidate cache by creating new Lazy instance
        Interlocked.Exchange(ref _lazyOptions, new Lazy<JsonSerializerOptions>(CreateOptions));
    }

    public static void RegisterTypeModifier<T>(Action<JsonTypeInfo> modifier)
    {
        // Atomically update the modifiers collection
        var wrappedModifier = new Action<JsonTypeInfo>(ti =>
        {
            if (ti.Type == typeof(T))
            {
                modifier(ti);
            }
        });
        
        ImmutableInterlocked.Update(ref _modifiers, list => list.Add(wrappedModifier));
        
        // Invalidate cache by creating new Lazy instance
        Interlocked.Exchange(ref _lazyOptions, new Lazy<JsonSerializerOptions>(CreateOptions));
    }
}