using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.MediatR.Attributes;

/// <summary>
/// A typed attribute bag backed by <see cref="JsonElement"/> values, providing
/// serialization-aware get and set operations using the Alexa JSON serializer options.
/// </summary>
public sealed class JsonAttributeBag
{
    private readonly Dictionary<string, JsonElement> _values;

    /// <summary>
    /// Initializes a new instance of <see cref="JsonAttributeBag"/> wrapping the provided dictionary.
    /// </summary>
    /// <param name="values">The underlying dictionary of raw JSON elements. Must not be <see langword="null"/>.</param>
    public JsonAttributeBag(Dictionary<string, JsonElement> values)
    {
        _values = values ?? throw new ArgumentNullException(nameof(values));
    }

    /// <summary>
    /// Gets the underlying dictionary of raw <see cref="JsonElement"/> values.
    /// </summary>
    public Dictionary<string, JsonElement> Values => _values;

    /// <summary>
    /// Gets or sets the raw <see cref="JsonElement"/> for the specified key.
    /// </summary>
    /// <param name="key">The attribute key.</param>
    public JsonElement this[string key]
    {
        get => _values[key];
        set => _values[key] = value;
    }

    /// <summary>
    /// Serializes <paramref name="value"/> and stores it under <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The attribute key.</param>
    /// <param name="value">The value to serialize and store.</param>
    public void Set<T>(string key, T? value) =>
        _values[key] = JsonSerializer.SerializeToElement(value, AlexaJsonOptions.DefaultOptions);

    /// <summary>
    /// Retrieves and deserializes the value for <paramref name="key"/>, or returns
    /// <see langword="default"/> if the key does not exist.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the value into.</typeparam>
    /// <param name="key">The attribute key.</param>
    /// <returns>The deserialized value, or <see langword="default"/> if not found.</returns>
    public T? Get<T>(string key) => _values.TryGetValue(key, out var element)
        ? element.Deserialize<T>(AlexaJsonOptions.DefaultOptions)
        : default;

    /// <summary>
    /// Attempts to retrieve and deserialize the value for <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the value into.</typeparam>
    /// <param name="key">The attribute key.</param>
    /// <param name="value">
    /// When this method returns <see langword="true"/>, contains the deserialized value;
    /// otherwise <see langword="default"/>.
    /// </param>
    /// <returns><see langword="true"/> if the key was found; otherwise <see langword="false"/>.</returns>
    public bool TryGet<T>(string key, out T? value)
    {
        value = default;

        if (!_values.TryGetValue(key, out var element))
            return false;

        value = element.Deserialize<T>(AlexaJsonOptions.DefaultOptions);
        return true;
    }

    /// <summary>
    /// Retrieves and deserializes the value for <paramref name="key"/>, throwing
    /// <see cref="KeyNotFoundException"/> if the key does not exist.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the value into.</typeparam>
    /// <param name="key">The attribute key.</param>
    /// <returns>The deserialized value.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when <paramref name="key"/> is not found.</exception>
    public T GetRequired<T>(string key)
    {
        return !TryGet<T>(key, out var value)
            ? throw new KeyNotFoundException($"Attribute '{key}' not found.")
            : value!;
    }

    /// <summary>
    /// Removes the attribute with the specified key.
    /// </summary>
    /// <param name="key">The attribute key to remove.</param>
    /// <returns><see langword="true"/> if the key was found and removed; otherwise <see langword="false"/>.</returns>
    public bool Remove(string key) => _values.Remove(key);

    /// <summary>
    /// Removes all attributes from the bag.
    /// </summary>
    public void Clear() => _values.Clear();
}