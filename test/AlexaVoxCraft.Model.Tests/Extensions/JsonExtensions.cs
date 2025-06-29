using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit.Sdk;

namespace AlexaVoxCraft.Model.Tests.Extensions;

public static class JsonExtensions
{
    public static JsonObject AsObjectNode(this JsonElement element)
    {
        return JsonNode.Parse(element.GetRawText())!.AsObject();
    }

    public static void AssertJsonEqual(this JsonElement actual, JsonElement expected, string? messagePrefix = null)
    {
        var diffs = new List<string>();

        if (!expected.JsonElementDeepEquals(actual, "", diffs))
        {
            var message = $"{messagePrefix ?? "❌ JSON mismatch:"}\n" +
                          $"Differences:\n- {string.Join("\n- ", diffs)}\n\n" +
                          $"Expected:\n{expected}\n\nActual:\n{actual}";
            throw new XunitException(message);
        }
    }

    public static void AssertJsonEqual<T>(this T actualObject, string expectedJson,
        JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions { WriteIndented = true };

        var actualJson = JsonSerializer.Serialize(actualObject, options);

        using var expectedDoc = JsonDocument.Parse(expectedJson);
        using var actualDoc = JsonDocument.Parse(actualJson);

        actualDoc.RootElement.AssertJsonEqual(expectedDoc.RootElement);
    }
    public static bool JsonDeepEquals<T>(
        this T? a,
        T? b,
        out List<string> differences,
        JsonSerializerOptions? options = null)
    {
        differences = [];

        if (a is null && b is null) return true;
        if (a is null || b is null)
        {
            differences.Add("One object is null while the other is not.");
            return false;
        }

        options ??= new JsonSerializerOptions { WriteIndented = false };

        var aJson = JsonSerializer.SerializeToElement(a, options);
        var bJson = JsonSerializer.SerializeToElement(b, options);

        return aJson.JsonElementDeepEquals(bJson, "", differences);
    }

    public static bool JsonDeepEquals<T>(
        this T? a,
        T? b,
        JsonSerializerOptions? options = null) =>
        a.JsonDeepEquals(b, out _, options);
}