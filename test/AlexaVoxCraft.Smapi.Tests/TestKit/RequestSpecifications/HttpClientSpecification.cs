using System.Reflection;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.RequestSpecifications;

/// <summary>
/// Specification that determines if a request is for an HttpClient type.
/// </summary>
public class HttpClientSpecification : IRequestSpecification
{
    /// <summary>
    /// Determines whether the request is satisfied by this specification.
    /// </summary>
    /// <param name="request">The request to evaluate.</param>
    /// <returns>True if the request is for an HttpClient type; otherwise, false.</returns>
    public bool IsSatisfiedBy(object request)
    {
        return request is Type type && type == typeof(HttpClient) ||
               (request is ParameterInfo parameter && parameter.ParameterType == typeof(HttpClient));
    }
}