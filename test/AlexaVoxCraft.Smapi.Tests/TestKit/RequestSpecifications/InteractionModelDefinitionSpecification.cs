using System.Reflection;
using AlexaVoxCraft.Smapi.Models.InteractionModel;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.RequestSpecifications;

/// <summary>
/// Specification that determines if a request is for an InteractionModelDefinition type.
/// </summary>
public class InteractionModelDefinitionSpecification : IRequestSpecification
{
    /// <summary>
    /// Determines whether the request is satisfied by this specification.
    /// </summary>
    /// <param name="request">The request to evaluate.</param>
    /// <returns>True if the request is for an InteractionModelDefinition type; otherwise, false.</returns>
    public bool IsSatisfiedBy(object request)
    {
        return request is Type type && type == typeof(InteractionModelDefinition) ||
               (request is ParameterInfo parameter && parameter.ParameterType == typeof(InteractionModelDefinition));
    }
}