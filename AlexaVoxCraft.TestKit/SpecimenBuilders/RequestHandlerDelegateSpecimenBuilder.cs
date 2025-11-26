using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;
using NSubstitute;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

/// <summary>
/// Custom specimen builder for RequestHandlerDelegate - creates valid instances for testing.
/// </summary>
public class RequestHandlerDelegateSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public RequestHandlerDelegateSpecimenBuilder() : this(new RequestHandlerDelegateSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        // Return a delegate that creates a valid SkillResponse
        var requestHandlerDelegate = Substitute.For<RequestHandlerDelegate>();
        var skillResponse = (SkillResponse)context.Resolve(typeof(SkillResponse));
        requestHandlerDelegate.Invoke().Returns(Task.FromResult(skillResponse));
        return requestHandlerDelegate;
    }
}