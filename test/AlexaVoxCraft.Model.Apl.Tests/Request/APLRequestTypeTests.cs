using System.Text.Json;
using AlexaVoxCraft.Model.Apl.DataStore;
using AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

namespace AlexaVoxCraft.Model.Apl.Tests.Request;

/// <summary>
/// Request-side coverage for the remaining APL/DataStore request types beyond
/// <see cref="UserEventRequest"/> (already covered in <c>APLSkillRequestTests.cs</c>): extension
/// list-loading, runtime errors, package-manager lifecycle events, and DataStore errors. All
/// request-side deserialize, fixtures built fresh.
/// </summary>
public sealed class APLRequestTypeTests() : TestBase<APLRequestTypeTests>
{
    [Fact]
    public async Task LoadIndexListDataRequest_Deserializes()
    {
        var request = Deserialize<LoadIndexListDataRequest>("Requests/LoadIndexListDataRequest.json");

        request.ListId.Should().Be("my-list-id");
        request.StartIndex.Should().Be(10);
        request.Count.Should().Be(5);

        await TestHelper.VerifyRequestObject(request);
    }

    [Fact]
    public async Task LoadTokenListDataRequest_Deserializes()
    {
        var request = Deserialize<LoadTokenListDataRequest>("Requests/LoadTokenListDataRequest.json");

        request.ListId.Should().Be("my-list-id");
        request.PageToken.Should().Be("next-page-token");

        await TestHelper.VerifyRequestObject(request);
    }

    [Fact]
    public async Task RuntimeErrorRequest_Deserializes()
    {
        var request = Deserialize<RuntimeErrorRequest>("Requests/RuntimeErrorRequest.json");

        request.Errors.Should().ContainSingle().Which.Reason.Should().Be("INVALID_INDEX");

        await TestHelper.VerifyRequestObject(request);
    }

    [Fact]
    public async Task UsagesInstalledRequest_Deserializes()
    {
        var request = Deserialize<UsagesInstalledRequest>("Requests/UsagesInstalledRequest.json");

        request.Payload.PackageId.Should().Be("WeatherWidget");
        request.Payload.PackageVersion.Should().Be("1.0.0");
        var usage = request.Payload.Usages.Should().ContainSingle().Subject;
        usage.InstanceId.Should().Be("amzn1.ask.package.v1.instance.v1.example");
        usage.Location.Should().Be("FAVORITE");

        await TestHelper.VerifyRequestObject(request);
    }

    [Fact]
    public async Task UsagesRemovedRequest_Deserializes()
    {
        var request = Deserialize<UsagesRemovedRequest>("Requests/UsagesRemovedRequest.json");

        request.Payload.PackageId.Should().Be("WeatherWidget");
        request.Payload.Usages.Should().ContainSingle();

        await TestHelper.VerifyRequestObject(request);
    }

    [Fact]
    public async Task UpdateRequest_Deserializes()
    {
        var request = Deserialize<UpdateRequest>("Requests/UpdateRequest.json");

        request.PackageId.Should().Be("WeatherWidget");
        request.FromVersion.Should().Be("1.0.0");
        request.ToVersion.Should().Be("1.0.1");

        await TestHelper.VerifyRequestObject(request);
    }

    [Fact]
    public async Task InstallationError_Deserializes()
    {
        var request = Deserialize<InstallationError>("Requests/InstallationError.json");

        request.PackageId.Should().Be("WeatherWidget");
        request.Version.Should().Be("1.0.0");
        request.Error.Type.Should().Be("PACKAGEMANAGER_INTERNAL_ERROR");

        await TestHelper.VerifyRequestObject(request);
    }

    [Fact]
    public async Task DataStoreErrorRequest_Deserializes_AsStorageError()
    {
        var request = Deserialize<DataStoreErrorRequest>("Requests/DataStoreError_Storage.json");

        var error = request.Error.Should().BeOfType<DataStoreStorageError>().Subject;
        error.Type.Should().Be("STORAGE_LIMIT_EXCEEDED");
        error.Content.DeviceId.Should().Be("device-id");
        var command = error.Content.FailedCommand.Should().BeOfType<PutObject>().Subject;
        command.Namespace.Should().Be("namespace-from-the-command");
        command.Key.Should().Be("key-from-the-command");

        await TestHelper.VerifyRequestObject(request);
    }

    [Fact]
    public async Task DataStoreErrorRequest_Deserializes_AsDeviceError()
    {
        var request = Deserialize<DataStoreErrorRequest>("Requests/DataStoreError_Device.json");

        var error = request.Error.Should().BeOfType<DataStoreDeviceError>().Subject;
        error.Type.Should().Be("DEVICE_UNAVAILABLE");
        error.Content.DeviceId.Should().Be("device-id");
        var command = error.Content.Commands.Should().ContainSingle().Subject;
        var putObject = command.Should().BeOfType<PutObject>().Subject;
        putObject.Namespace.Should().Be("namespace-for-the-command");

        await TestHelper.VerifyRequestObject(request);
    }

    private static T Deserialize<T>(string fixturePath)
    {
        var json = Fx(fixturePath);
        var request = JsonSerializer.Deserialize<T>(json, AlexaJson);
        request.Should().NotBeNull();
        return request!;
    }
}
