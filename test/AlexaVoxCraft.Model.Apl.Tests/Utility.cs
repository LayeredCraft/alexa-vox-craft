using System.Text.Json;
using System.Text.Json.Nodes;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Tests;

public static class Utility
{
    private const string ExamplesPath = "Examples";

    public static bool CompareJson(object actual, string expectedFile, ITestOutputHelper? output,
        params string[] exclude)
    {
        // Read expected JSON directly as JsonNode
        var expectedJson = File.ReadAllText(Path.Combine(ExamplesPath, expectedFile));
        var expectedNode = JsonSerializer.Deserialize<JsonNode>(expectedJson, AlexaJsonOptions.DefaultOptions);

        // Serialize actual object directly into JsonNode
        var actualNode = JsonSerializer.SerializeToNode(actual, AlexaJsonOptions.DefaultOptions);

        // Remove properties by paths (supporting dot notation)
        actualNode!.RemovePaths(exclude);
        expectedNode!.RemovePaths(exclude);

        var actualString = actualNode.ToJsonString();
        var expectedString = expectedNode.ToJsonString();
        
        // Re-parse into JsonDocuments
        using var cleanedActual = JsonDocument.Parse(actualNode.ToJsonString());
        using var cleanedExpected = JsonDocument.Parse(expectedNode.ToJsonString());

        // Compare with deep diff
        var diffs = new List<string>();
        var equal = cleanedActual.RootElement.JsonElementDeepEquals(cleanedExpected.RootElement, "", diffs);  
        
        if (!equal && output is not null)
        {
            output.WriteLine("❌ JSON DeepEquals failed. Differences:");
            foreach (var diff in diffs)
            {
                output.WriteLine($"• {diff}");
            }
        }

        return equal;
        //
        // var actualJObject = JObject.FromObject(actual);
        // var expected = File.ReadAllText(Path.Combine(ExamplesPath, expectedFile));
        // var expectedJObject = JObject.Parse(expected);
        //
        // foreach (var item in exclude)
        // {
        //     RemoveFrom(actualJObject, item);
        //     RemoveFrom(expectedJObject, item);
        // }
        //
        // var result = JToken.DeepEquals(expectedJObject, actualJObject);
        //
        // if (!result)
        // {
        //     OutputTrimEqual(expectedJObject, actualJObject);
        // }
        //
        // return result;
    }

    /// <summary>
    /// Removes multiple paths (dot-separated) from the root JsonNode.
    /// </summary>
    public static void RemovePaths(this JsonNode? root, params string[] paths)
    {
        foreach (var path in paths)
        {
            root.RemovePath(path);
        }
    }
    
    /// <summary>
    /// Removes a single path (dot-separated) from the JsonNode.
    /// </summary>
    public static void RemovePath(this JsonNode? node, string path)
    {
        if (node is null || string.IsNullOrWhiteSpace(path))
            return;

        var segments = path.Split('.');
        RemovePathRecursive(node, segments, 0);
    }

    private static void RemovePathRecursive(JsonNode? current, string[] segments, int index)
    {
        if (current is null || index >= segments.Length)
            return;

        if (current is JsonObject obj)
        {
            var key = segments[index];

            if (index == segments.Length - 1)
            {
                obj.Remove(key);
                return;
            }

            if (obj.TryGetPropertyValue(key, out var nextNode))
            {
                RemovePathRecursive(nextNode, segments, index + 1);
            }
        }
        else if (current is JsonArray array)
        {
            foreach (var element in array)
            {
                RemovePathRecursive(element, segments, index);
            }
        }
    }
    public static T ExampleFileContent<T>(string expectedFile)
    {
        var json = ExampleFileContent(expectedFile);
        return JsonSerializer.Deserialize<T>(json, AlexaJsonOptions.DefaultOptions);
    }

    public static string ExampleFileContent(string expectedFile)
    {
        return File.ReadAllText(Path.Combine(ExamplesPath, expectedFile));
    }

    public static T AssertComponent<T>(string expectedFile, ITestOutputHelper? output = null) where T : APLComponent
    {
        var component = AssertSerialization<T>(expectedFile, output);
        if (component.Properties != null)
        {
            component.Properties.Remove("type");
            Assert.Empty(component.Properties);
        }
        else
        {
            Assert.Null(component.Properties);
        }

        return component;
    }

    public static T AssertSerialization<T>(string expectedFile, ITestOutputHelper? output = null)
    {
        var obj = ExampleFileContent<T>(expectedFile);
        Assert.True(CompareJson(obj, expectedFile, output));
        return obj;
    }

    public static void AssertSerialization<TImplied, TExplicit>(string expectedFile, ITestOutputHelper? output = null)
    {
        var obj = ExampleFileContent<TImplied>(expectedFile);
        var final = Assert.IsType<TExplicit>(obj);
        Assert.True(CompareJson(final, expectedFile, output));
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