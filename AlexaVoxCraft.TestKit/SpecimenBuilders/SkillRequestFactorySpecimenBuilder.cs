using System.Reflection;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;
using NSubstitute;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

/// <summary>
/// Custom specimen builder for SkillRequest - creates valid instances for testing.
/// </summary>
public class SkillRequestFactorySpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public SkillRequestFactorySpecimenBuilder() : this(new SkillRequestFactorySpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        // Return a factory that creates a valid SkillRequest
        return Substitute.For<SkillRequestFactory>();
    }
}
