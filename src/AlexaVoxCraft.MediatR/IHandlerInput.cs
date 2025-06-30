using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Response;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR;

public interface IHandlerInput
{
    SkillRequest RequestEnvelope { get; }
    
    IAttributesManager AttributesManager { get; }
    
    IResponseBuilder ResponseBuilder { get; }
}