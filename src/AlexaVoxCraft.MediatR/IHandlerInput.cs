using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Response;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR;

/// <summary>
/// Provides access to request data and utilities needed for processing Alexa skill requests.
/// This interface encapsulates the core dependencies that request handlers typically need
/// when processing incoming Alexa skill requests.
/// </summary>
public interface IHandlerInput
{
    /// <summary>
    /// Gets the complete Alexa skill request envelope containing the request, session, and context information.
    /// </summary>
    /// <value>The skill request envelope received from the Alexa service.</value>
    SkillRequest RequestEnvelope { get; }
    
    /// <summary>
    /// Gets the attributes manager for reading and writing persistent and session attributes.
    /// Use this to maintain state across skill sessions and requests.
    /// </summary>
    /// <value>The attributes manager instance for state management.</value>
    IAttributesManager AttributesManager { get; }
    
    /// <summary>
    /// Gets the response builder for constructing Alexa skill responses.
    /// Use this to build speech output, cards, directives, and other response elements.
    /// </summary>
    /// <value>The response builder instance for creating skill responses.</value>
    IResponseBuilder ResponseBuilder { get; }
}