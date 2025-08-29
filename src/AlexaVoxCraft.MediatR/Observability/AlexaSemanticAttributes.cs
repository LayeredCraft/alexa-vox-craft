namespace AlexaVoxCraft.MediatR.Observability;

public static class AlexaSemanticAttributes
{
    public const string RpcSystem = "rpc.system";
    public const string RpcService = "rpc.service";
    public const string RpcMethod = "rpc.method";
    
    public const string FaasColdStart = "faas.coldstart";
    public const string ColdStart = "cold_start";
    
    public const string RequestType = "alexa.request.type";
    public const string IntentName = "alexa.intent.name";
    public const string SessionNew = "alexa.session.new";
    public const string SessionId = "alexa.session.id";
    public const string Locale = "alexa.locale";
    public const string DeviceHasScreen = "alexa.device.has_screen";
    public const string DialogState = "alexa.dialog.state";
    public const string ApplicationId = "alexa.application.id";
    public const string RequestId = "alexa.request.id";

    public const string HandlerType = "alexa.handler.type";
    public const string HandlerCanHandle = "alexa.handler.can_handle";
    public const string HandlerExecutionOrder = "alexa.handler.execution_order";
    public const string HandlerIsDefault = "alexa.handler.is_default";
    public const string CodeNamespace = "code.namespace";
    public const string CodeFunction = "code.function";
    
    public const string ErrorType = "alexa.error.type";
    public const string ExceptionType = "exception.type";
    public const string ExceptionMessage = "exception.message";
    public const string ExceptionStackTrace = "exception.stacktrace";
    
    public const string ResponseHasCard = "alexa.response.has_card";
    public const string ResponseHasApl = "alexa.response.has_apl";
    public const string ResponseHasReprompt = "alexa.response.has_reprompt";
    public const string ResponseShouldEndSession = "alexa.response.should_end_session";

    public const string SlotName = "alexa.slot.name";
    public const string SlotResolutionStatus = "alexa.slot.resolution.status";
    
    public const string SpeechCharacters = "speech_chars";
    public const string RepromptCharacters = "reprompt_chars";
    
    public const string SerializationDirection = "alexa.serialization.direction";
    public const string PayloadSize = "alexa.payload.size";
    
    public const string FaasName = "faas.name";
    public const string FaasVersion = "faas.version";
    public const string AwsLambdaRequestId = "aws.lambda.request_id";
    public const string AwsLambdaMemoryLimit = "aws.lambda.memory_limit";
    public const string AwsLambdaRemainingTime = "aws.lambda.remaining_time";
}