using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using AlexaVoxCraft.Model.Serialization;
using Serilog.Core;
using Serilog.Events;

namespace AlexaVoxCraft.Lambda.Serialization;

/// <summary>
/// Serilog destructuring policy that uses System.Text.Json for object serialization.
/// </summary>
public class SystemTextDestructuringPolicy : IDestructuringPolicy
{
    /// <summary>
    /// Attempts to destructure an object for Serilog logging using System.Text.Json serialization.
    /// </summary>
    /// <param name="value">The object to destructure.</param>
    /// <param name="propertyValueFactory">Factory for creating log event property values.</param>
    /// <param name="result">The destructured log event property value.</param>
    /// <returns>Always returns true as this policy handles all objects.</returns>
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory,
        [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        var jsonNode = JsonSerializer.SerializeToNode(value, AlexaJsonOptions.DefaultOptions);
        result = ConvertToLogEventPropertyValue(jsonNode);
        return true;
    }

    private LogEventPropertyValue ConvertToLogEventPropertyValue(JsonNode? node)
    {
        return node switch
        {
            JsonObject obj => new StructureValue(obj.Select(kvp =>
                new LogEventProperty(kvp.Key, ConvertToLogEventPropertyValue(kvp.Value)))),
            JsonArray array => new SequenceValue(array.Select(ConvertToLogEventPropertyValue)),
            JsonValue val => ConvertJsonValue(val),
            _ => new ScalarValue(null)
        };
    }

    private static LogEventPropertyValue ConvertJsonValue(JsonValue value)
    {
        if (value.TryGetValue(out bool b)) return new ScalarValue(b);
        if (value.TryGetValue(out int i)) return new ScalarValue(i);
        if (value.TryGetValue(out long l)) return new ScalarValue(l);
        if (value.TryGetValue(out double d)) return new ScalarValue(d);
        if (value.TryGetValue(out string? s)) return new ScalarValue(s);
        if (value.TryGetValue(out DateTime dt)) return new ScalarValue(dt);
        if (value.TryGetValue(out Guid g)) return new ScalarValue(g);
        return value.TryGetValue(out JsonElement je) ?
            // Fall back to string representation
            new ScalarValue(je.ToString()) : new ScalarValue(value.ToJsonString());
    }
}