using System.Reflection;
using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.SpecimenBuilders;

public sealed class SkillRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var isMatch = request switch
        {
            Type type => type == typeof(SkillRequest),
            ParameterInfo parameter => parameter.ParameterType == typeof(SkillRequest),
            _ => false
        };

        if (!isMatch)
            return new NoSpecimen();

        return new SkillRequest
        {
            Version = "1.0",
            Session = new Session { Attributes = new Dictionary<string, JsonElement>() },
            Context = new Context { System = new AlexaSystem { Application = new Application { ApplicationId = "amzn1.ask.skill.test-skill-id" } } },
            Request = new LaunchRequest
            {
                Type = "LaunchRequest",
                RequestId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Locale = "en-US"
            }
        };
    }
}
