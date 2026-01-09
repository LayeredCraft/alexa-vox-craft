using AlexaVoxCraft.Model.Apl.Extensions.Backstack;
using AlexaVoxCraft.Model.Apl.Extensions.DataStore;
using AlexaVoxCraft.Model.Apl.Extensions.EntitySensing;
using AlexaVoxCraft.Model.Apl.Extensions.SmartMotion;
using AlexaVoxCraft.Model.Apl.Legacy.Tests.Extensions;

namespace AlexaVoxCraft.Model.Apl.Legacy.Tests;

public class ExtensionTests
{
    private readonly ITestOutputHelper _output;

    public ExtensionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void BackStackExtension()
    {
        var backstack = new BackstackExtension("Back");
        var doc = new APLDocument(APLDocumentVersion.V1_4);
        doc.Extensions!.Items!.Add(backstack);
        doc.Settings = new APLDocumentSettings();
        doc.Settings.Add(backstack.Name, new BackStackSettings { BackstackId = "myDocument" });
        Assert.True(Utility.CompareJson(doc, "ExtensionBackStack.json", _output));
    }

    [Fact]
    public void BackStackGoBack()
    {
        var expected = """
                       {
                           "type": "Back:GoBack"
                       }
                       """;

        var goBack = GoBackCommand.For(new BackstackExtension("Back"));
        goBack.AssertJsonEqual(expected);
    }

    [Fact]
    public void BackStackGoBackFull()
    {
        var expected = """
                       {
                           "type": "Back:GoBack",
                           "backType": "id",
                           "backValue": "myDocument"
                       }
                       """;

        var goBack = GoBackCommand.For(new BackstackExtension("Back"));
        goBack.BackType = BackTypeKind.Id;
        goBack.BackValue = "myDocument";

        goBack.AssertJsonEqual(expected);
    }

    [Fact]
    public void BackstackClear()
    {
        var expexted = """
                       {
                            "type": "Back:Clear"
                       }
                       """;
        var clear = ClearCommand.For(new BackstackExtension("Back"));
        clear.AssertJsonEqual(expexted);
    }

    [Fact]
    public void SmartMotion()
    {
        var smartMotion = new SmartMotionExtension("SmartMotion");
        var doc = new APLDocument(APLDocumentVersion.V1_4);
        doc.Extensions!.Items!.Add(smartMotion);
        doc.Settings = new APLDocumentSettings();
        doc.Settings.Add(smartMotion.Name, new SmartMotionSettings
        {
            DeviceStateName = "MyDeviceState",
            WakeWordResponse = WakeWordResponse.FollowOnWakeWord
        });
        Assert.True(Utility.CompareJson(doc, "ExtensionSmartMotion.json", null));
    }

    [Fact]
    public void FollowPrimaryUser()
    {
        var expected = """ 
                       {
                           "type": "SmartMotion:FollowPrimaryUser"
                       }
                       """;
            
        var followPrimaryUser = FollowPrimaryUserCommand.For(new SmartMotionExtension("SmartMotion"));
        followPrimaryUser.AssertJsonEqual(expected);
    }

    [Fact]
    public void GoToCenter()
    {
        var expected = """
                       {
                           "type": "SmartMotion:GoToCenter"
                       }
                       """; 
        var goToCenter = GoToCenterCommand.For(new SmartMotionExtension("SmartMotion"));
        goToCenter.AssertJsonEqual(expected);
    }

    [Fact]
    public void SetWakeWordResponse()
    {
        var expected = """
                          {
                            "type": "SmartMotion:SetWakeWordResponse",
                            "wakeWordResponse": "turnToWakeWord"
                          }
                       """;
        var setWake = SetWakeWordResponseCommand.For(new SmartMotionExtension("SmartMotion"));
        setWake.WakeWordResponse = WakeWordResponse.TurnToWakeWord;
        setWake.AssertJsonEqual(expected);
    }

    [Fact]
    public void StopMotion()
    {
        var expected = """
                       {
                           "type": "SmartMotion:StopMotion"
                       }
                       """; 
        var stopMotion = StopMotionCommand.For(new SmartMotionExtension("SmartMotion"));
        stopMotion.AssertJsonEqual(expected);
    }

    [Fact]
    public void TurnToPrimaryUser()
    {
        var expected = """
                       {
                           "type": "SmartMotion:TurnToPrimaryUser"
                       }
                       """; 
        var turnToPrimaryUser = TurnToPrimaryUserCommand.For(new SmartMotionExtension("SmartMotion"));
        turnToPrimaryUser.AssertJsonEqual(expected);
    }

    [Fact]
    public void PlayNamedChoreo()
    {
        var expected = """
                       {
                           "type": "SmartMotion:PlayNamedChoreo",
                           "name": "ScreenImpactCenter"
                       }
                       """; 
        var playNamedChoreo = PlayNamedChoreoCommand.For(new SmartMotionExtension("SmartMotion"), "ScreenImpactCenter");
        playNamedChoreo.AssertJsonEqual(expected);
    }

    [Fact]
    public void DeviceStateChangedHandler()
    {
        var doc = new APLDocument();
        var smartMotion = new SmartMotionExtension("SmartMotion");
        smartMotion.OnDeviceStateChanged(doc, null);
        Assert.True(doc.Handlers.ContainsKey("SmartMotion:OnDeviceStateChanged"));
    }

    [Fact]
    public void EntitySensing()
    {
        var entitySensing = new EntitySensingExtension("EntitySensing");
        var doc = new APLDocument(APLDocumentVersion.V1_4);
        doc.Extensions!.Items!.Add(entitySensing);
        doc.Settings = new APLDocumentSettings();
        doc.Settings.Add(entitySensing.Name, new EntitySensingSettings
        {
            EntitySensingStateName = "EntitySensingState",
            PrimaryUserName = "User"
        });
        Assert.True(Utility.CompareJson(doc, "ExtensionEntitySensing.json", _output));
    }

    [Fact]
    public void EntitySensingStateChanged()
    {
        var doc = new APLDocument();
        var entitySensing = new EntitySensingExtension("EntitySensing");
        entitySensing.OnEntitySensingStateChanged(doc, null);
        Assert.True(doc.Handlers.ContainsKey("EntitySensing:OnEntitySensingStateChanged"));
    }

    [Fact]
    public void PrimaryUserChanged()
    {
        var doc = new APLDocument();
        var entitySensing = new EntitySensingExtension("EntitySensing");
        entitySensing.OnPrimaryUserChanged(doc, null);
        Assert.True(doc.Handlers.ContainsKey("EntitySensing:OnPrimaryUserChanged"));
    }

    [Fact]
    public void DataStore()
    {
        var dataStore = new DataStoreExtension("DataStore");
        var doc = new APLDocument(APLDocumentVersion.V2023_1);
        doc.Extensions!.Items!.Add(dataStore);
        doc.Settings = new APLDocumentSettings();
        doc.Settings.Add(dataStore.Name, new DataStoreSettings
        {
            DataBindings = new()
            {
                new DataBinding
                {
                    Namespace = "LocationWeather",
                    Key = "weather",
                    DataBindingName = "DS_Weather"
                }
            }
        });
        Assert.True(Utility.CompareJson(doc, "DataStore_Extension.json", null));
    }

    [Fact]
    public void DataStoreGetObject()
    {
        var expected = """
                       {
                           "type": "DataStore:GetObject",
                           "namespace": "LocationWeather",
                           "key": "weather",
                           "token": "thisIsADummyToken"
                       }
                       """; 
        var getObject = GetObjectCommand.For(new DataStoreExtension("DataStore"), "LocationWeather",
            "weather", "thisIsADummyToken");
        getObject.AssertJsonEqual(expected);
    }

    [Fact]
    public void DataStoreWatchObject()
    {
        var expected = """
                       {
                           "type": "DataStore:WatchObject",
                           "namespace": "LocationWeather",
                           "key": "weather"
                       }
                       """; 
        var watchObject = WatchObjectCommand.For(new DataStoreExtension("DataStore"), "LocationWeather", "weather");
        watchObject.AssertJsonEqual(expected);
    }

    [Fact]
    public void DataStoreUnwatchObject()
    {
        var expected = """
                       {
                           "type": "DataStore:UnwatchObject",
                           "namespace": "LocationWeather",
                           "key": "weather"
                       }
                       """; 
        var unwatchObject = UnwatchObjectCommand.For(new DataStoreExtension("DataStore"), "LocationWeather", "weather");
        unwatchObject.AssertJsonEqual(expected);
    }

    [Fact]
    public void DataStoreUpdateArrayBindingRange()
    {
        var expected = """
                       {
                           "type": "DataStore:UpdateArrayBindingRange",
                           "dataBindingName": "ToDoNotes",
                           "startIndex": "${test}",
                           "endIndex": 5
                       }
                       """; 
        var updateArrayBindingRange = UpdateArrayBindingRangeCommand.For(new DataStoreExtension("DataStore"), "ToDoNotes",
            APLValue.To<int?>("${test}"), 5);
        updateArrayBindingRange.AssertJsonEqual(expected);
    }

    [Fact]
    public void DataStoreOnObjectChanged()
    {
        var doc = new APLDocument();
        var entitySensing = new DataStoreExtension("DataStore");
        entitySensing.OnObjectChanged(doc, null);
        Assert.True(doc.Handlers.ContainsKey("DataStore:OnObjectChanged"));
    }

    [Fact]
    public void DataStoreOnObjectReceived()
    {
        var doc = new APLDocument();
        var entitySensing = new DataStoreExtension("DataStore");
        entitySensing.OnObjectReceived(doc, null);
        Assert.True(doc.Handlers.ContainsKey("DataStore:OnObjectReceived"));
    }
}