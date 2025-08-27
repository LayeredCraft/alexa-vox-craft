namespace AlexaVoxCraft.MediatR.Observability;

public static class AlexaMetricNames
{
    public const string Requests = "alexa.requests";
    public const string Errors = "alexa.errors";
    public const string ColdStarts = "alexa.cold_starts";
    public const string SlotResolutions = "alexa.slots.resolution";
    public const string SkillVerificationFailures = "alexa.skill_verification.failures";
    
    public const string Latency = "alexa.latency";
    public const string HandlerDuration = "alexa.handler.duration";
    public const string SerializationDuration = "alexa.serialization.duration";
    public const string ResponseSize = "alexa.response.size_bytes";
    public const string SpeechCharacters = "alexa.response.speech.characters";
    public const string AplRenderDuration = "alexa.apl.render.duration";
    public const string PayloadSize = "alexa.payload.size";
}