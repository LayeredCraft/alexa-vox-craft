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

    public const string SlotResolutionMatch = "match";
    public const string SlotResolutionNoMatch = "no_match";
    public const string SlotResolutionError = "error";
    
    public const string SerializationDirectionRequest = "request";
    public const string SerializationDirectionResponse = "response";
}