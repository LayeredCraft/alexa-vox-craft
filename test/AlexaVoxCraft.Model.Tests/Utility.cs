using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;
using JsonElement = System.Text.Json.JsonElement;
using JsonValueKind = System.Text.Json.JsonValueKind;

namespace AlexaVoxCraft.Model.Tests;

public static class Utility
{
    private const string ExamplesPath = "Examples";
    private static JsonSerializerOptions? Options = new(AlexaJsonOptions.DefaultOptions);

    public static bool CompareJson(object actual, string expectedFile)
    {

        // Serialize the actual object to JSON
        var actualJson = JsonSerializer.Serialize(actual, Options);
        using var actualDoc = JsonDocument.Parse(actualJson);

        // Read and parse the expected JSON from file
        var expectedJson = File.ReadAllText(Path.Combine(ExamplesPath, expectedFile));
        using var expectedDoc = JsonDocument.Parse(expectedJson);
        var diffs = new List<string>();
        return actualDoc.RootElement.JsonElementDeepEquals(expectedDoc.RootElement, "", diffs);
    }

    public static T ExampleFileContent<T>(string expectedFile)
    {
        var json = ExampleFileContent(expectedFile);
        return JsonSerializer.Deserialize<T>(json, Options)!;
    }

    public static string ExampleFileContent(string expectedFile)
    {
        return File.ReadAllText(Path.Combine(ExamplesPath, expectedFile));
    }

    public static bool JsonElementDeepEquals(this JsonElement a, JsonElement b, string path = "",
        List<string>? diffs = null)
    {
        diffs ??= [];

        if (a.ValueKind != b.ValueKind)
        {
            diffs.Add($"{path}: Type mismatch ({a.ValueKind} != {b.ValueKind})");
            return false;
        }

        switch (a.ValueKind)
        {
            case JsonValueKind.Object:
                var aProps = a.EnumerateObject().OrderBy(p => p.Name).ToList();
                var bProps = b.EnumerateObject().OrderBy(p => p.Name).ToList();
                if (aProps.Count != bProps.Count)
                {
                    diffs.Add($"{path}: Property count mismatch");
                    return false;
                }

                for (var i = 0; i < aProps.Count; i++)
                {
                    var name = aProps[i].Name;
                    var subPath = string.IsNullOrEmpty(path) ? name : $"{path}.{name}";
                    if (name != bProps[i].Name ||
                        !JsonElementDeepEquals(aProps[i].Value, bProps[i].Value, subPath, diffs))
                        return false;
                }

                return true;

            case JsonValueKind.Array:
                var aArray = a.EnumerateArray().ToList();
                var bArray = b.EnumerateArray().ToList();
                if (aArray.Count != bArray.Count)
                {
                    diffs.Add($"{path}: Array length mismatch");
                    return false;
                }

                return !aArray.Where((t, i) => !JsonElementDeepEquals(t, bArray[i], $"{path}[{i}]", diffs)).Any();

            default:
                var equal = a.ToString() == b.ToString();
                if (!equal)
                {
                    diffs.Add($"{path}: Value mismatch '{a}' != '{b}'");
                }

                return equal;
        }
    }
}