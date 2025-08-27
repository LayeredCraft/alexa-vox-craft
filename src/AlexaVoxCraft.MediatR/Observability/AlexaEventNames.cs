namespace AlexaVoxCraft.MediatR.Observability;

public static class AlexaEventNames
{
    public const string SlotResolution = "alexa.slots.resolution";
    public const string ResponseBuilt = "alexa.response.built";
    public const string AplRenderStart = "alexa.apl.render.start";
    public const string AplRenderEnd = "alexa.apl.render.end";
    public const string HandlerResolved = "alexa.handler.resolved";
    public const string ContextCreated = "alexa.context.created";
}