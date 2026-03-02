using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.MediatR.Attributes;

public sealed class JsonAttributeBag
{
    private readonly Dictionary<string, JsonElement> _values;
    private readonly JsonSerializerOptions _options = AlexaJsonOptions.DefaultOptions;

    public JsonAttributeBag(Dictionary<string, JsonElement> values)
    {
        _values = values ?? throw new ArgumentNullException(nameof(values));
    }

    public Dictionary<string, JsonElement> Values => _values;

    public JsonElement this[string key]
    {
        get => _values[key];
        set => _values[key] = value;
    }

    public void Set<T>(string key, T? value) => _values[key] = JsonSerializer.SerializeToElement(value, _options);

    public T? Get<T>(string key) => _values.TryGetValue(key, out var element)
        ? element.Deserialize<T>(_options)
        : default;

    public bool TryGet<T>(string key, out T? value)
    {
        value = default;

        if (!_values.TryGetValue(key, out var element))
            return false;

        value = element.Deserialize<T>(_options);
        return true;
    }

    public T GetRequired<T>(string key)
    {
        return !TryGet<T>(key, out var value)
            ? throw new KeyNotFoundException($"Attribute '{key}' not found.")
            : value!;
    }

    public bool Remove(string key) => _values.Remove(key);

    public void Clear() => _values.Clear();
}