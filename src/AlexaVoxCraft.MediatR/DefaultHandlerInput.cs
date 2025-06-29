using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Response;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR;

public class DefaultHandlerInput : IHandlerInput
{
    public DefaultHandlerInput(SkillRequestFactory skillRequestFactory, IAttributesManager attributesManager, IResponseBuilder responseBuilder)
    {
        ArgumentNullException.ThrowIfNull(skillRequestFactory);
        RequestEnvelope = skillRequestFactory() ?? throw new ArgumentNullException(nameof(RequestEnvelope));
        AttributesManager = attributesManager ?? throw new ArgumentNullException(nameof(attributesManager));
        ResponseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
    }
    public SkillRequest RequestEnvelope { get; }
    public IAttributesManager AttributesManager { get; }
    public IResponseBuilder ResponseBuilder { get; }
}