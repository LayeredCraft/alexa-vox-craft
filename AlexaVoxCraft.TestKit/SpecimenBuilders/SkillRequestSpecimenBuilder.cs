using System.Reflection;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

/// <summary>
/// Custom specimen builder for SkillRequest - creates valid instances for testing.
/// </summary>
public class SkillRequestSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public SkillRequestSpecimenBuilder() : this(new SkillRequestSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        // Create real objects instead of substitutes to avoid virtual member issues
        var application = new Application { ApplicationId = "amzn1.ask.skill.test-skill-id" };
        var system = new AlexaSystem { Application = application };
        var skillContext = new Context { System = system };
        var session = new Session { Attributes = new Dictionary<string, object>() };

        var skillRequest = new SkillRequest 
        { 
            Context = skillContext,
            Request = (Request)context.Resolve(typeof(Request)),
            Session = session
        };
        
        return skillRequest;
    }
}
