using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Apl.Extensions.Backstack;
using AlexaVoxCraft.Model.Apl.Extensions.DataStore;
using AlexaVoxCraft.Model.Apl.Extensions.EntitySensing;
using AlexaVoxCraft.Model.Apl.Extensions.SmartMotion;

namespace AlexaVoxCraft.Model.Apl.Tests.Document;

/// <summary>
/// Coverage for the bespoke APL extensions (BackStack, SmartMotion, EntitySensing, DataStore): their
/// document-level registration/settings and their commands. All response-side, serialize-only,
/// consistent with every other APL type in this project.
/// </summary>
public sealed class ExtensionTests() : TestBase<ExtensionTests>
{
    [Fact]
    public async Task BackStackExtension_Serializes_OnDocument()
    {
        var backstack = new BackstackExtension("Back");
        var doc = new APLDocument(APLDocumentVersion.V1_4) { MainTemplate = new Layout(new Text { Content = "Question text" }) };
        doc.Extensions!.Add(backstack);
        doc.Settings = new APLDocumentSettings();
        doc.Settings.Add(backstack.Name!, new BackStackSettings { BackstackId = "myDocument" });

        await TestHelper.VerifySerializedObject(doc, AlexaJson, "Document_WithBackStackExtension");
    }

    [Fact]
    public async Task BackStackGoBack_Serializes()
    {
        var goBack = GoBackCommand.For(new BackstackExtension("Back"));

        await TestHelper.VerifySerializedObject(goBack, AlexaJson, "BackStack_GoBack");
    }

    [Fact]
    public async Task BackStackGoBack_Serializes_WithBackTypeAndValue()
    {
        var goBack = GoBackCommand.For(new BackstackExtension("Back"));
        goBack.BackType = BackTypeKind.Id;
        goBack.BackValue = "myDocument";

        await TestHelper.VerifySerializedObject(goBack, AlexaJson, "BackStack_GoBack_WithBackTypeAndValue");
    }

    [Fact]
    public async Task BackStackClear_Serializes()
    {
        var clear = ClearCommand.For(new BackstackExtension("Back"));

        await TestHelper.VerifySerializedObject(clear, AlexaJson, "BackStack_Clear");
    }

    [Fact]
    public async Task SmartMotionExtension_Serializes_OnDocument()
    {
        var smartMotion = new SmartMotionExtension("SmartMotion");
        var doc = new APLDocument(APLDocumentVersion.V1_4) { MainTemplate = new Layout(new Text { Content = "Question text" }) };
        doc.Extensions!.Add(smartMotion);
        doc.Settings = new APLDocumentSettings();
        doc.Settings.Add(smartMotion.Name!, new SmartMotionSettings
        {
            DeviceStateName = "MyDeviceState",
            WakeWordResponse = WakeWordResponse.FollowOnWakeWord
        });

        await TestHelper.VerifySerializedObject(doc, AlexaJson, "Document_WithSmartMotionExtension");
    }

    [Fact]
    public async Task SmartMotionFollowPrimaryUser_Serializes()
    {
        var command = FollowPrimaryUserCommand.For(new SmartMotionExtension("SmartMotion"));

        await TestHelper.VerifySerializedObject(command, AlexaJson, "SmartMotion_FollowPrimaryUser");
    }

    [Fact]
    public async Task SmartMotionGoToCenter_Serializes()
    {
        var command = GoToCenterCommand.For(new SmartMotionExtension("SmartMotion"));

        await TestHelper.VerifySerializedObject(command, AlexaJson, "SmartMotion_GoToCenter");
    }

    [Fact]
    public async Task SmartMotionSetWakeWordResponse_Serializes()
    {
        var command = SetWakeWordResponseCommand.For(new SmartMotionExtension("SmartMotion"));
        command.WakeWordResponse = WakeWordResponse.TurnToWakeWord;

        await TestHelper.VerifySerializedObject(command, AlexaJson, "SmartMotion_SetWakeWordResponse");
    }

    [Fact]
    public async Task SmartMotionStopMotion_Serializes()
    {
        var command = StopMotionCommand.For(new SmartMotionExtension("SmartMotion"));

        await TestHelper.VerifySerializedObject(command, AlexaJson, "SmartMotion_StopMotion");
    }

    [Fact]
    public async Task SmartMotionTurnToPrimaryUser_Serializes()
    {
        var command = TurnToPrimaryUserCommand.For(new SmartMotionExtension("SmartMotion"));

        await TestHelper.VerifySerializedObject(command, AlexaJson, "SmartMotion_TurnToPrimaryUser");
    }

    [Fact]
    public async Task SmartMotionPlayNamedChoreo_Serializes()
    {
        var command = PlayNamedChoreoCommand.For(new SmartMotionExtension("SmartMotion"), "ScreenImpactCenter");

        await TestHelper.VerifySerializedObject(command, AlexaJson, "SmartMotion_PlayNamedChoreo");
    }

    [Fact]
    public void SmartMotionDeviceStateChanged_RegistersHandler()
    {
        var doc = new APLDocument();
        var smartMotion = new SmartMotionExtension("SmartMotion");

        smartMotion.OnDeviceStateChanged(doc, null);

        doc.Handlers.Should().ContainKey("SmartMotion:OnDeviceStateChanged");
    }

    [Fact]
    public async Task EntitySensingExtension_Serializes_OnDocument()
    {
        var entitySensing = new EntitySensingExtension("EntitySensing");
        var doc = new APLDocument(APLDocumentVersion.V1_4) { MainTemplate = new Layout(new Text { Content = "Question text" }) };
        doc.Extensions!.Add(entitySensing);
        doc.Settings = new APLDocumentSettings();
        doc.Settings.Add(entitySensing.Name!, new EntitySensingSettings
        {
            EntitySensingStateName = "EntitySensingState",
            PrimaryUserName = "User"
        });

        await TestHelper.VerifySerializedObject(doc, AlexaJson, "Document_WithEntitySensingExtension");
    }

    [Fact]
    public void EntitySensingStateChanged_RegistersHandler()
    {
        var doc = new APLDocument();
        var entitySensing = new EntitySensingExtension("EntitySensing");

        entitySensing.OnEntitySensingStateChanged(doc, null);

        doc.Handlers.Should().ContainKey("EntitySensing:OnEntitySensingStateChanged");
    }

    [Fact]
    public void EntitySensingPrimaryUserChanged_RegistersHandler()
    {
        var doc = new APLDocument();
        var entitySensing = new EntitySensingExtension("EntitySensing");

        entitySensing.OnPrimaryUserChanged(doc, null);

        doc.Handlers.Should().ContainKey("EntitySensing:OnPrimaryUserChanged");
    }

    [Fact]
    public async Task DataStoreExtension_Serializes_OnDocument()
    {
        var dataStore = new DataStoreExtension("DataStore");
        var doc = new APLDocument(APLDocumentVersion.V2023_1) { MainTemplate = new Layout(new Text { Content = "Question text" }) };
        doc.Extensions!.Add(dataStore);
        doc.Settings = new APLDocumentSettings();
        doc.Settings.Add(dataStore.Name!, new DataStoreSettings
        {
            DataBindings =
            [
                new DataBinding { Namespace = "LocationWeather", Key = "weather", DataBindingName = "DS_Weather" }
            ]
        });

        await TestHelper.VerifySerializedObject(doc, AlexaJson, "Document_WithDataStoreExtension");
    }

    [Fact]
    public async Task DataStoreGetObject_Serializes()
    {
        var command = GetObjectCommand.For(new DataStoreExtension("DataStore"), "LocationWeather", "weather", "thisIsADummyToken");

        await TestHelper.VerifySerializedObject(command, AlexaJson, "DataStore_GetObject");
    }

    [Fact]
    public async Task DataStoreWatchObject_Serializes()
    {
        var command = WatchObjectCommand.For(new DataStoreExtension("DataStore"), "LocationWeather", "weather");

        await TestHelper.VerifySerializedObject(command, AlexaJson, "DataStore_WatchObject");
    }

    [Fact]
    public async Task DataStoreUnwatchObject_Serializes()
    {
        var command = UnwatchObjectCommand.For(new DataStoreExtension("DataStore"), "LocationWeather", "weather");

        await TestHelper.VerifySerializedObject(command, AlexaJson, "DataStore_UnwatchObject");
    }

    [Fact]
    public async Task DataStoreUpdateArrayBindingRange_Serializes()
    {
        var command = UpdateArrayBindingRangeCommand.For(new DataStoreExtension("DataStore"), new APLValue<string>("ToDoNotes"), APLValue.To<int?>("${test}"), new APLValue<int?>(5));

        await TestHelper.VerifySerializedObject(command, AlexaJson, "DataStore_UpdateArrayBindingRange");
    }

    [Fact]
    public void DataStoreOnObjectChanged_RegistersHandler()
    {
        var doc = new APLDocument();
        var dataStore = new DataStoreExtension("DataStore");

        dataStore.OnObjectChanged(doc, null);

        doc.Handlers.Should().ContainKey("DataStore:OnObjectChanged");
    }

    [Fact]
    public void DataStoreOnObjectReceived_RegistersHandler()
    {
        var doc = new APLDocument();
        var dataStore = new DataStoreExtension("DataStore");

        dataStore.OnObjectReceived(doc, null);

        doc.Handlers.Should().ContainKey("DataStore:OnObjectReceived");
    }
}
