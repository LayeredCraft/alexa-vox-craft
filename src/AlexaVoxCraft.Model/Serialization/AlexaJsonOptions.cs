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

    // Package-owned resolvers (AplSupport.Add(), InSkillPurchasingSupport.Add(), Smapi bootstrap) always
    // precede consumer-registered resolvers in the composed chain, regardless of call order relative to
    // consumer registrations - this is an internal-only registration surface, never exposed publicly.
    private static ImmutableArray<IJsonTypeInfoResolver> _packageResolvers = ImmutableArray<IJsonTypeInfoResolver>.Empty;

    // Consumer-registered resolvers (public RegisterTypeInfoResolver). Always placed after package
    // resolvers and before the JIT fallback in the composed chain - this ordering is structural, not a
    // function of registration call order.
    private static ImmutableArray<IJsonTypeInfoResolver> _consumerResolvers = ImmutableArray<IJsonTypeInfoResolver>.Empty;

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
        // Structural chain order: Model -> package-owned contexts -> consumer resolvers -> JIT fallback.
        // This order is fixed by CreateOptions() itself, not by the relative order in which
        // RegisterTypeInfoResolver/internal package registration were called at runtime.
        var resolvers = new List<IJsonTypeInfoResolver> { ModelContext.Default };
        resolvers.AddRange(_packageResolvers);
        resolvers.AddRange(_consumerResolvers);

        if (JsonSerializer.IsReflectionEnabledByDefault)
        {
            resolvers.Add(new DefaultJsonTypeInfoResolver());
        }

        IJsonTypeInfoResolver resolver = JsonTypeInfoResolver.Combine(resolvers.ToArray());

        // Reproduces AlexaTypeResolver's three hard-coded ShouldSerialize behaviors as one more modifier
        // applied ahead of registered modifiers on the outermost combined resolver, preserving today's
        // effective ordering (these ran inside AlexaTypeResolver.GetTypeInfo, i.e. logically first).
        resolver = resolver.WithAddedModifier(BuiltInShouldSerializeModifier);
        resolver = resolver.WithAddedModifier(Modifiers.SetNumberHandlingModifier);

        // Add all registered modifiers thread-safely
        foreach (var modifier in _modifiers)
        {
            resolver = resolver.WithAddedModifier(modifier);
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

    private static void BuiltInShouldSerializeModifier(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Type == typeof(Response.ResponseBody))
        {
            var prop = typeInfo.Properties.FirstOrDefault(p => p.Name == "directives");
            prop?.ShouldSerialize = (obj, _) =>
            {
                var response = (Response.ResponseBody)obj;
                return response.Directives is { Count: > 0 };
            };
        }
        else if (typeInfo.Type == typeof(Response.Reprompt))
        {
            var prop = typeInfo.Properties.FirstOrDefault(p => p.Name == "directives");
            prop?.ShouldSerialize = (obj, _) =>
            {
                var response = (Response.Reprompt)obj;
                return response.Directives is { Count: > 0 };
            };
        }
        else if (typeInfo.Type == typeof(Response.Directive.Templates.ImageSource))
        {
            var widthProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "widthPixels");
            widthProp?.ShouldSerialize = (obj, _) =>
            {
                var imageSource = (Response.Directive.Templates.ImageSource)obj;
                return imageSource.Width > 0;
            };

            var heightProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "heightPixels");
            heightProp?.ShouldSerialize = (obj, _) =>
            {
                var imageSource = (Response.Directive.Templates.ImageSource)obj;
                return imageSource.Height > 0;
            };
        }
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

    /// <summary>
    /// Registers a consumer-owned <see cref="IJsonTypeInfoResolver"/> (typically a source-generated
    /// <see cref="JsonSerializerContext"/>'s <c>.Default</c> instance) so its types participate in
    /// <see cref="DefaultOptions"/>'s resolver chain. Consumer resolvers are always placed after every
    /// AlexaVoxCraft package-owned context and before the JIT reflection fallback, regardless of when
    /// this method is called relative to package bootstrap calls (e.g. <c>AplSupport.Add()</c>).
    /// </summary>
    public static void RegisterTypeInfoResolver(IJsonTypeInfoResolver resolver)
    {
        ArgumentNullException.ThrowIfNull(resolver);

        lock (_lock)
        {
            _consumerResolvers = _consumerResolvers.Add(resolver);
            _version++;
            _cachedOptions = null;
        }
    }

    /// <summary>
    /// Internal registration surface for AlexaVoxCraft's own package-owned contexts
    /// (<c>AplModelContext</c>, <c>InSkillPurchasingModelContext</c>, <c>SmapiModelContext</c>). Always
    /// placed ahead of consumer-registered resolvers in <see cref="DefaultOptions"/>'s chain, independent
    /// of call order between package bootstrap and consumer registration.
    /// </summary>
    internal static void RegisterPackageTypeInfoResolver(IJsonTypeInfoResolver resolver)
    {
        ArgumentNullException.ThrowIfNull(resolver);

        lock (_lock)
        {
            _packageResolvers = _packageResolvers.Add(resolver);
            _version++;
            _cachedOptions = null;
        }
    }
}
