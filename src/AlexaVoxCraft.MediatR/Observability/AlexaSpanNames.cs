namespace AlexaVoxCraft.MediatR.Observability;

public static class AlexaSpanNames
{
    public const string LambdaExecution = "alexa.lambda.execution";
    public const string Request = "alexa.request";
    public const string Handler = "alexa.handler";
    public const string HandlerResolution = "alexa.handler.resolution";
    public const string SerializationRequest = "alexa.serialization.request";
    public const string SerializationResponse = "alexa.serialization.response";
    public const string AplRender = "alexa.apl.render";
    public const string Mediation = "alexa.mediation";
}