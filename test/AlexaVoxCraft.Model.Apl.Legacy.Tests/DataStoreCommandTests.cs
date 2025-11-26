using System.Text.Json;
using AlexaVoxCraft.Model.Apl.DataStore;
using AlexaVoxCraft.Model.Serialization;
using JsonDocument = System.Text.Json.JsonDocument;

namespace AlexaVoxCraft.Model.Apl.Legacy.Tests;

public class DataStoreCommandTests
{
    [Fact]
    public void PutNamespaceCommand()
    {
        var rawJson = """
                      {
                          "namespace": "test",
                          "type": "PUT_NAMESPACE"
                      }
                      """;

        var cmd = JsonSerializer.Deserialize<DataStoreCommand>(rawJson, AlexaJsonOptions.DefaultOptions);
        var typedCmd = Assert.IsType<PutNamespace>(cmd);

        Assert.Equal("test", typedCmd.Namespace);

        var serialized = JsonSerializer.Serialize(typedCmd, AlexaJsonOptions.DefaultOptions);
        var reparsed = JsonDocument.Parse(serialized).RootElement;
        var original = JsonDocument.Parse(rawJson).RootElement;

        Assert.True(reparsed.JsonElementDeepEquals(original), "Serialized JSON does not match original input.");
    }

    [Fact]
    public void RemoveNamespaceCommand()
    {
        var rawJson = """
                      {
                          "namespace": "test",
                          "type": "REMOVE_NAMESPACE"
                      }
                      """;

        var cmd = JsonSerializer.Deserialize<DataStoreCommand>(rawJson, AlexaJsonOptions.DefaultOptions);
        var typedCmd = Assert.IsType<RemoveNamespace>(cmd);

        Assert.Equal("test", typedCmd.Namespace);

        var serialized = JsonSerializer.Serialize(typedCmd, AlexaJsonOptions.DefaultOptions);
        var reparsed = JsonDocument.Parse(serialized).RootElement;
        var original = JsonDocument.Parse(rawJson).RootElement;

        Assert.True(reparsed.JsonElementDeepEquals(original), "Serialized JSON does not match original input.");
    }

    [Fact]
    public void ClearCommand()
    {
        var rawJson = """
                      {
                          "type": "CLEAR"
                      }
                      """;

        var cmd = JsonSerializer.Deserialize<DataStoreCommand>(rawJson, AlexaJsonOptions.DefaultOptions);
        var typedCmd = Assert.IsType<Clear>(cmd);

        var serialized = JsonSerializer.Serialize(typedCmd, AlexaJsonOptions.DefaultOptions);
        var reparsed = JsonDocument.Parse(serialized).RootElement;
        var original = JsonDocument.Parse(rawJson).RootElement;

        Assert.True(reparsed.JsonElementDeepEquals(original), "Serialized JSON does not match original input.");
    }

    [Fact]
    public void PutObjectCommand()
    {
        var raw = Utility.ExampleFileContent<DataStoreCommand>("DataStore_PutObject.json");
        var cmd = Assert.IsType<PutObject>(raw);
        Utility.CompareJson(cmd, "DataStore_PutObject.json", null);
    }

    [Fact]
    public void PutObjectArrayCommand()
    {
        var raw = Utility.ExampleFileContent<DataStoreCommand>("DataStore_PutObjectArray.json");
        var cmd = Assert.IsType<PutObjectArray>(raw);
        Utility.CompareJson(cmd, "DataStore_PutObjectArray.json", null);
    }
}