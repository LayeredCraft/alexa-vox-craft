using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using AlexaVoxCraft.Model.Serialization;
using Serilog.Core;
using Serilog.Events;

namespace AlexaVoxCraft.MediatR.Lambda.Serialization;

public class SystemTextDestructuringPolicy : IDestructuringPolicy
{
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