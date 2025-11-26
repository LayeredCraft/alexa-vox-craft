using AlexaVoxCraft.Model.Apl.DataStore;
using AlexaVoxCraft.Model.Apl.DataStore.PackageManager;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.Apl.Legacy.Tests;

public class RequestTests
{
    private readonly ITestOutputHelper _output;

    public RequestTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void LoadIndexListData()
    {
        var req = Utility.ExampleFileContent<Request.Type.Request>("LoadIndexListDataRequest.json");
        var loadIndex = Assert.IsType<LoadIndexListDataRequest>(req);
        Assert.True(Utility.CompareJson(loadIndex, "LoadIndexListDataRequest.json", _output));
    }

    [Fact]
    public void LoadTokenListData()
    {
        var req = Utility.ExampleFileContent<Request.Type.Request>("LoadTokenListDataRequest.json");
        var loadIndex = Assert.IsType<LoadTokenListDataRequest>(req);
        Assert.True(Utility.CompareJson(loadIndex, "LoadTokenListDataRequest.json", null));
    }

    [Fact]
    public void RuntimeError()
    {
        var req = Utility.ExampleFileContent<Request.Type.Request>("RuntimeError.json");
        var errorReq = Assert.IsType<RuntimeErrorRequest>(req);
        Assert.True(Utility.CompareJson(errorReq, "RuntimeError.json", null));
    }

    [Fact]
    public void UsageInstalled()
    {
        var req = Utility.ExampleFileContent<Request.Type.Request>("UsagesInstalledRequest.json");
        var installedRequest = Assert.IsType<UsagesInstalledRequest>(req);

        Assert.Equal("WeatherWidget", installedRequest.Payload.PackageId);
        Assert.Equal("1.0.0", installedRequest.Payload.PackageVersion);

        var usage = Assert.Single(installedRequest.Payload.Usages);
        Assert.Equal("amzn1.ask.package.v1.instance.v1.{uuid}",usage.InstanceId);
        Assert.Equal("FAVORITE",usage.Location);
    }

    [Fact]
    public void UsageRemoved()
    {
        var req = Utility.ExampleFileContent<Request.Type.Request>("UsagesRemovedRequest.json");
        var removedRequest = Assert.IsType<UsagesRemovedRequest>(req);

        Assert.Equal("WeatherWidget", removedRequest.Payload.PackageId);
        Assert.Equal("1.0.0", removedRequest.Payload.PackageVersion);

        var usage = Assert.Single(removedRequest.Payload.Usages);
        Assert.Equal("amzn1.ask.package.v1.instance.v1.{uuid}", usage.InstanceId);
        Assert.Equal("FAVORITE", usage.Location);
    }

    [Fact]
    public void UsageUpdate()
    {
        var req = Utility.ExampleFileContent<Request.Type.Request>("UsageUpdateRequest.json");
        var removedRequest = Assert.IsType<UpdateRequest>(req);

        Assert.Equal("WeatherWidget", removedRequest.PackageId);
        Assert.Equal("1.0.0", removedRequest.FromVersion);
        Assert.Equal("1.0.1", removedRequest.ToVersion);
    }

    [Fact]
    public void InstallationError()
    {
        var req = Utility.ExampleFileContent<Request.Type.Request>("InstallationError.json");
        var err = Assert.IsType<InstallationError>(req);

        Assert.Equal("WeatherWidget", err.PackageId);
        Assert.Equal("1.0.0", err.Version);
        Assert.Equal("PACKAGEMANAGER_INTERNAL_ERROR", err.Error.Type);
    }

    [Fact]
    public void DataStoreErrorStorage()
    {
        var req = Utility.ExampleFileContent<Request.Type.Request>("DataStoreError_Storage.json");
        var typedReq = Assert.IsType<DataStoreErrorRequest>(req);
        var err = Assert.IsType<DataStoreStorageError>(typedReq.Error);

        Assert.Equal("STORAGE_LIMIT_EXCEEDED", err.Type);
        Assert.Equal("device-id", err.Content.DeviceId);
        var cmd = Assert.IsType<PutObject>(err.Content.FailedCommand);
        Assert.Equal("namespace-from-the-command", cmd.Namespace);
        Assert.Equal("key-from-the-command", cmd.Key);
    }

    [Fact]
    public void DataStoreErrorDevice()
    {
        var req = Utility.ExampleFileContent<Request.Type.Request>("DataStoreError_Device.json");
        var typedReq = Assert.IsType<DataStoreErrorRequest>(req);
        var err = Assert.IsType<DataStoreDeviceError>(typedReq.Error);

        Assert.Equal("DEVICE_UNAVAILABLE", err.Type);
        Assert.Equal("device-id", err.Content.DeviceId);
        var cmd = Assert.Single(err.Content.Commands);
        var cnt = Assert.IsType<PutObject>(cmd);
        Assert.Equal("namespace-for-the-command", cnt.Namespace);
        Assert.Equal("key-from-the-command", cnt.Key);
    }

    [Fact]
    public void UserEventRequest()
    {
        var req = Utility.ExampleFileContent<Request.Type.Request>("TouchEvent.json");
        var typedReq = Assert.IsType<UserEventRequest>(req);
        Assert.Equal(2, typedReq.Arguments!.Length);
        var touchWrapper = Assert.IsType<APLCommandSource>(typedReq.Source);
        Assert.Equal("Press", touchWrapper.Handler);
    }
    
    [Fact]
    public void CanReadSessionAttributes()
    {
        var request = Utility.ExampleFileContent<SkillRequest>("RequestWithSession.json");
        Assert.NotNull(request.Session.Attributes);
    }

}