using System.Text.Json;
using AlexaVoxCraft.Model.Apl.DataStore;

namespace AlexaVoxCraft.Model.Apl.Tests.DataStore;

/// <summary>
/// Coverage for <see cref="DataStoreCommand"/> types. Commands are sent by the skill to the
/// DataStore service via <see cref="DataStoreClient.Commands"/>, so — like every other response-side
/// type in this project — they're tested via serialize, not deserialize (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class DataStoreCommandTests() : TestBase<DataStoreCommandTests>
{
    [Fact]
    public async Task PutNamespace_Serializes()
    {
        var command = new PutNamespace { Namespace = "test" };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "PutNamespace");
    }

    [Fact]
    public async Task RemoveNamespace_Serializes()
    {
        var command = new RemoveNamespace { Namespace = "test" };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "RemoveNamespace");
    }

    [Fact]
    public async Task Clear_Serializes()
    {
        var command = new Clear();

        await TestHelper.VerifySerializedObject(command, AlexaJson, "Clear");
    }

    [Fact]
    public async Task PutObject_Serializes()
    {
        var command = new PutObject
        {
            Namespace = "namespace-from-the-command",
            Key = "key-from-the-command",
            Content = JsonSerializer.SerializeToElement(new { score = 42 })
        };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "PutObject");
    }

    [Fact]
    public async Task PutObjectArray_Serializes()
    {
        var command = new PutObjectArray
        {
            Namespace = "namespace-from-the-command",
            Key = "key-from-the-command",
            Content = [JsonSerializer.SerializeToElement("first"), JsonSerializer.SerializeToElement("second")]
        };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "PutObjectArray");
    }
}
