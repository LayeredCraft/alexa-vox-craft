using System.Reflection;
using AlexaVoxCraft.Smapi.Tests.TestKit.RequestSpecifications;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.SpecimenBuilders;

/// <summary>
/// AutoFixture specimen builder that creates configured HttpClient instances for SMAPI testing.
/// </summary>
public sealed class HttpClientSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientSpecimenBuilder"/> class
    /// with the default HttpClientSpecification.
    /// </summary>
    public HttpClientSpecimenBuilder() : this(new HttpClientSpecification())
    {

    }

    /// <summary>
    /// Creates an HttpClient instance with the SMAPI base address and a frozen HttpMessageHandler.
    /// </summary>
    /// <param name="request">The specimen request.</param>
    /// <param name="context">The specimen context.</param>
    /// <returns>A configured HttpClient instance, or NoSpecimen if the request doesn't match.</returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        var parameterName = request switch
        {
            ParameterInfo parameter => parameter.Name?.ToLowerInvariant() ?? "",
            Type type => "",
            _ => throw new ArgumentException("Invalid request type", nameof(request))
        };

        var handler = context.Resolve(typeof(HttpMessageHandler)) as HttpMessageHandler;
        var httpClient = new HttpClient(handler!)
        {
            BaseAddress = new Uri("https://api.amazonalexa.com/")
        };
        return httpClient;
    }
}