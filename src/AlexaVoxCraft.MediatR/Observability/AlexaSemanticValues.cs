namespace AlexaVoxCraft.MediatR.Observability;

public static class AlexaSemanticValues
{
    public const string RpcSystemAlexa = "alexa";
    
    public const string True = "true";
    public const string False = "false";
    
    public const string ErrorTypeValidation = "validation";
    public const string ErrorTypeBusiness = "business";
    public const string ErrorTypeUnhandled = "unhandled";
    public const string ErrorTypeTimeout = "timeout";
    public const string ErrorTypeSkillVerification = "skill_verification";
    
    public const string SlotResolutionMatch = "match";
    public const string SlotResolutionNoMatch = "no_match";
    public const string SlotResolutionError = "error";
    
    public const string SerializationDirectionRequest = "request";
    public const string SerializationDirectionResponse = "response";
    
    public const string LaunchRequest = "LaunchRequest";
    public const string IntentRequest = "IntentRequest";
    public const string SessionEndedRequest = "SessionEndedRequest";
    public const string UserEventRequest = "UserEventRequest";
    
    public const string DialogStateStarted = "STARTED";
    public const string DialogStateInProgress = "IN_PROGRESS";
    public const string DialogStateCompleted = "COMPLETED";
}