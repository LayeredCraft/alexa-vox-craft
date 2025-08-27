namespace AlexaVoxCraft.MediatR.Observability;

public static class AlexaSemanticAttributes
{
    public const string RpcSystem = "rpc.system";
    public const string RpcService = "rpc.service";
    public const string RpcMethod = "rpc.method";
    
    public const string RequestType = "alexa.request.type";
    public const string IntentName = "alexa.intent.name";
    public const string SessionNew = "alexa.session.new";
    public const string SessionId = "alexa.session.id";
    public const string Locale = "alexa.locale";
    public const string DeviceHasScreen = "alexa.device.has_screen";
    public const string DialogState = "alexa.dialog.state";
    public const string SkillId = "alexa.skill.id";
    public const string ApplicationId = "alexa.application.id";
    public const string RequestId = "alexa.request.id";
    public const string UserId = "alexa.user.id";
    
    public const string HandlerType = "alexa.handler.type";
    public const string CodeNamespace = "code.namespace";
    public const string CodeFunction = "code.function";
    
    public const string ErrorType = "alexa.error.type";
    public const string ExceptionType = "exception.type";
    
    public const string ResponseHasCard = "alexa.response.has_card";
    public const string ResponseHasApl = "alexa.response.has_apl";
    public const string ResponseHasReprompt = "alexa.response.has_reprompt";
    public const string ResponseShouldEndSession = "alexa.response.should_end_session";
    
    public const string AplDocumentType = "alexa.apl.document.type";
    public const string AplComponentCount = "alexa.apl.component.count";
    
    public const string SlotName = "alexa.slot.name";
    public const string SlotResolutionStatus = "alexa.slot.resolution.status";
}