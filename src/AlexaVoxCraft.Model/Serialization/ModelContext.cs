using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.ConnectionTasks.Inputs;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.Serialization;

// Root types only: everything reachable from a root's own properties is generated automatically
// by the source generator. Only types that are ever passed to JsonSerializer.Serialize/Deserialize
// directly by their concrete runtime Type (SkillRequest/SkillResponse themselves, and every concrete
// type a polymorphic converter can resolve to via System.Type) need an explicit entry here.
[JsonSerializable(typeof(SkillRequest))]
[JsonSerializable(typeof(SkillResponse))]
// Request-side polymorphic targets (RequestConverter / IRequestTypeResolver implementations)
[JsonSerializable(typeof(IntentRequest))]
[JsonSerializable(typeof(LaunchRequest))]
[JsonSerializable(typeof(SessionEndedRequest))]
[JsonSerializable(typeof(SystemExceptionRequest))]
[JsonSerializable(typeof(AudioPlayerRequest))]
[JsonSerializable(typeof(PlaybackControllerRequest))]
[JsonSerializable(typeof(DisplayElementSelectedRequest))]
[JsonSerializable(typeof(AccountLinkSkillEventRequest))]
[JsonSerializable(typeof(PermissionSkillEventRequest))]
[JsonSerializable(typeof(SkillEnablementSkillEventRequest))]
[JsonSerializable(typeof(SkillEventRequest))]
[JsonSerializable(typeof(SessionResumedRequest))]
[JsonSerializable(typeof(AskForPermissionRequest))]
// Directive-side polymorphic targets (DirectiveConverter)
[JsonSerializable(typeof(AudioPlayerPlayDirective))]
[JsonSerializable(typeof(ClearQueueDirective))]
[JsonSerializable(typeof(DialogConfirmIntent))]
[JsonSerializable(typeof(DialogConfirmSlot))]
[JsonSerializable(typeof(DialogDelegate))]
[JsonSerializable(typeof(DialogElicitSlot))]
[JsonSerializable(typeof(HintDirective))]
[JsonSerializable(typeof(StopDirective))]
[JsonSerializable(typeof(VideoAppDirective))]
[JsonSerializable(typeof(StartConnectionDirective))]
[JsonSerializable(typeof(CompleteTaskDirective))]
[JsonSerializable(typeof(DialogUpdateDynamicEntities))]
[JsonSerializable(typeof(AskForPermissionDirective))]
[JsonSerializable(typeof(JsonDirective))]
// ProgressiveResponseDirectiveConverter
[JsonSerializable(typeof(VoicePlayerSpeakDirective))]
// CardConverter
[JsonSerializable(typeof(SimpleCard))]
[JsonSerializable(typeof(StandardCard))]
[JsonSerializable(typeof(LinkAccountCard))]
[JsonSerializable(typeof(AskForPermissionsConsentCard))]
// OutputSpeechConverter
[JsonSerializable(typeof(SsmlOutputSpeech))]
[JsonSerializable(typeof(PlainTextOutputSpeech))]
// ConnectionTaskConverter
[JsonSerializable(typeof(PrintPdfV1))]
[JsonSerializable(typeof(PrintImageV1))]
[JsonSerializable(typeof(PrintWebPageV1))]
[JsonSerializable(typeof(ScheduleTaxiReservation))]
[JsonSerializable(typeof(ScheduleFoodEstablishmentReservation))]
internal partial class ModelContext : JsonSerializerContext
{
}
