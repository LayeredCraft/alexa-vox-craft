using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Directive.Templates;
using AlexaVoxCraft.Model.Response.Ssml;

namespace AlexaVoxCraft.Model.Serialization;

/// <summary>
/// JSON source generation context for all core Alexa skill types.
/// This provides compile-time JSON serialization for improved Lambda cold start performance.
/// </summary>
[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
// Core request/response envelope types
[JsonSerializable(typeof(SkillRequest))]
[JsonSerializable(typeof(SkillResponse))]
[JsonSerializable(typeof(ResponseBody))]
// Core request types
[JsonSerializable(typeof(Request.Type.Request))]
[JsonSerializable(typeof(LaunchRequest))]
[JsonSerializable(typeof(IntentRequest))]
[JsonSerializable(typeof(SessionEndedRequest))]
[JsonSerializable(typeof(SessionResumedRequest))]
[JsonSerializable(typeof(SystemExceptionRequest))]
[JsonSerializable(typeof(DisplayElementSelectedRequest))]
// Audio player requests
[JsonSerializable(typeof(AudioPlayerRequest))]
[JsonSerializable(typeof(PlaybackControllerRequest))]
// Connection and skill event requests
[JsonSerializable(typeof(ConnectionResponseRequest))]
[JsonSerializable(typeof(AskForPermissionRequest))]
[JsonSerializable(typeof(AskForPermissionRequestPayload))]
[JsonSerializable(typeof(SkillEventRequest))]
[JsonSerializable(typeof(AccountLinkSkillEventRequest))]
[JsonSerializable(typeof(AccountLinkSkillEventDetail))]
[JsonSerializable(typeof(PermissionSkillEventRequest))]
[JsonSerializable(typeof(SkillEnablementSkillEventRequest))]
// Request supporting types
[JsonSerializable(typeof(Intent))]
[JsonSerializable(typeof(Slot))]
[JsonSerializable(typeof(SlotValue))]
[JsonSerializable(typeof(Context))]
[JsonSerializable(typeof(Session))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(Application))]
[JsonSerializable(typeof(Device))]
[JsonSerializable(typeof(AlexaSystem))]
[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(Unit))]
[JsonSerializable(typeof(Error))]
[JsonSerializable(typeof(GeolocationCoordinate))]
[JsonSerializable(typeof(GeolocationAltitude))]
[JsonSerializable(typeof(GeolocationHeading))]
[JsonSerializable(typeof(GeolocationSpeed))]
[JsonSerializable(typeof(LocationServices))]
[JsonSerializable(typeof(PlaybackState))]
[JsonSerializable(typeof(LaunchRequestTask))]
[JsonSerializable(typeof(Permission))]
[JsonSerializable(typeof(Permissions))]
[JsonSerializable(typeof(SkillEventPermissions))]
// Card types
[JsonSerializable(typeof(ICard))]
[JsonSerializable(typeof(SimpleCard))]
[JsonSerializable(typeof(StandardCard))]
[JsonSerializable(typeof(LinkAccountCard))]
[JsonSerializable(typeof(AskForPermissionsConsentCard))]
[JsonSerializable(typeof(CardImage))]
// Output speech types
[JsonSerializable(typeof(IOutputSpeech))]
[JsonSerializable(typeof(PlainTextOutputSpeech))]
[JsonSerializable(typeof(SsmlOutputSpeech))]
[JsonSerializable(typeof(Reprompt))]
// Directive types
[JsonSerializable(typeof(IDirective))]
[JsonSerializable(typeof(AudioPlayerPlayDirective))]
[JsonSerializable(typeof(StopDirective))]
[JsonSerializable(typeof(ClearQueueDirective))]
[JsonSerializable(typeof(DialogDelegate))]
[JsonSerializable(typeof(DialogConfirmIntent))]
[JsonSerializable(typeof(DialogConfirmSlot))]
[JsonSerializable(typeof(DialogElicitSlot))]
[JsonSerializable(typeof(DialogUpdateDynamicEntities))]
[JsonSerializable(typeof(HintDirective))]
[JsonSerializable(typeof(VideoAppDirective))]
[JsonSerializable(typeof(VoicePlayerSpeakDirective))]
[JsonSerializable(typeof(StartConnectionDirective))]
[JsonSerializable(typeof(CompleteTaskDirective))]
[JsonSerializable(typeof(AskForPermissionDirective))]
[JsonSerializable(typeof(JsonDirective))]
// Directive supporting types
[JsonSerializable(typeof(AudioItem))]
[JsonSerializable(typeof(AudioItemStream))]
[JsonSerializable(typeof(AudioItemMetadata))]
[JsonSerializable(typeof(AudioItemSource))]
[JsonSerializable(typeof(AudioItemSources))]
[JsonSerializable(typeof(VideoItem))]
[JsonSerializable(typeof(VideoItemMetadata))]
[JsonSerializable(typeof(Hint))]
[JsonSerializable(typeof(ConnectionSendRequest))]
[JsonSerializable(typeof(AskForPermissionPayload))]
[JsonSerializable(typeof(ListItem))]
[JsonSerializable(typeof(SlotType))]
[JsonSerializable(typeof(SlotTypeValue))]
[JsonSerializable(typeof(SlotTypeValueName))]
// SSML types
[JsonSerializable(typeof(ISsml))]
[JsonSerializable(typeof(Speech))]
[JsonSerializable(typeof(Audio))]
[JsonSerializable(typeof(PlainText))]
[JsonSerializable(typeof(Prosody))]
[JsonSerializable(typeof(Voice))]
[JsonSerializable(typeof(Lang))]
[JsonSerializable(typeof(Paragraph))]
[JsonSerializable(typeof(Sentence))]
[JsonSerializable(typeof(AmazonEmotion))]
[JsonSerializable(typeof(AmazonDomain))]
[JsonSerializable(typeof(Break))]
[JsonSerializable(typeof(Emphasis))]
[JsonSerializable(typeof(Phoneme))]
[JsonSerializable(typeof(SayAs))]
[JsonSerializable(typeof(Sub))]
[JsonSerializable(typeof(WordRole))]
// Template types
[JsonSerializable(typeof(ImageSource))]
// Progressive response types
[JsonSerializable(typeof(ProgressiveResponse))]
[JsonSerializable(typeof(ProgressiveResponseRequest))]
[JsonSerializable(typeof(ProgressiveResponseHeader))]
// Enum types for proper serialization
[JsonSerializable(typeof(AudioRequestType))]
[JsonSerializable(typeof(PlaybackControllerRequestType))]
[JsonSerializable(typeof(ErrorType))]
[JsonSerializable(typeof(ErrorCause))]
[JsonSerializable(typeof(Reason))]
[JsonSerializable(typeof(LocationServiceAccess))]
[JsonSerializable(typeof(LocationServiceStatus))]
[JsonSerializable(typeof(PersistenceStatus))]
[JsonSerializable(typeof(SkillEventPersistenceStatus))]
[JsonSerializable(typeof(SessionResumedRequestCause))]
[JsonSerializable(typeof(PlayBehavior))]
[JsonSerializable(typeof(ClearBehavior))]
[JsonSerializable(typeof(UpdateBehavior))]
[JsonSerializable(typeof(OnCompleteAction))]
public partial class AlexaJsonContext : JsonSerializerContext
{
}