using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Extensions.DataStore;

public class WatchObjectCommand : APLCommand
{
    private readonly string _extensionName;

    public static WatchObjectCommand For(DataStoreExtension extension)
    {
        return new WatchObjectCommand(extension.Name);
    }

    public static WatchObjectCommand For(DataStoreExtension extension, string @namespace, string key)
    {
        return new WatchObjectCommand(extension.Name, @namespace, key);
    }

    public WatchObjectCommand(string extensionName)
    {
        _extensionName = extensionName;
    }

    public WatchObjectCommand(string extensionName, string @namespace, string key) : this(extensionName)
    {
        Namespace = @namespace;
        Key = key;
    }

    [JsonPropertyName("namespace")] public string Namespace { get; set; }

    [JsonPropertyName("key")] public string Key { get; set; }

    [JsonPropertyName("type")]
    public override string Type => $"{_extensionName}:WatchObject";
}