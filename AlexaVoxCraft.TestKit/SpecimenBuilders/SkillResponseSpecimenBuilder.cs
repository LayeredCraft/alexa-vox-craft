using System.Reflection;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

/// <summary>
/// Custom specimen builder for SkillResponse based on parameter naming conventions.
/// </summary>
public class SkillResponseSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public SkillResponseSpecimenBuilder() : this(new SkillResponseSpecification())
    {
    }
    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        // Create real SkillResponse object instead of substitute
        var skillResponse = new SkillResponse();
        return skillResponse;
    }
}