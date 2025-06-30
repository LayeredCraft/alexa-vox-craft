using System.Reflection;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public class SkillServiceConfigurationSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public SkillServiceConfigurationSpecimenBuilder() : this(new SkillServiceConfigurationSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        var parameterName = request switch
        {
            ParameterInfo parameter => parameter.Name?.ToLowerInvariant() ?? "",
            _ => ""
        };

        return parameterName switch
        {
            _ when parameterName.Contains("invalid") || parameterName.Contains("mismatched") => CreateInvalidConfiguration(),
            _ when parameterName.Contains("valid") || parameterName.Contains("matching") => CreateValidConfiguration(),
            _ when parameterName.Contains("empty") => CreateEmptySkillIdConfiguration(),
            _ when parameterName.Contains("null") => CreateNullSkillIdConfiguration(),
            _ when parameterName.Contains("whitespace") => CreateWhitespaceSkillIdConfiguration(),
            _ => CreateDefaultConfiguration()
        };
    }

    private static SkillServiceConfiguration CreateValidConfiguration()
    {
        return new SkillServiceConfiguration
        {
            SkillId = "amzn1.ask.skill.test-skill-id",
            CustomUserAgent = "TestAgent/1.0"
        };
    }

    private static SkillServiceConfiguration CreateInvalidConfiguration()
    {
        return new SkillServiceConfiguration
        {
            SkillId = "different-skill-id",
            CustomUserAgent = "TestAgent/1.0"
        };
    }

    private static SkillServiceConfiguration CreateEmptySkillIdConfiguration()
    {
        return new SkillServiceConfiguration
        {
            SkillId = "",
            CustomUserAgent = "TestAgent/1.0"
        };
    }

    private static SkillServiceConfiguration CreateNullSkillIdConfiguration()
    {
        return new SkillServiceConfiguration
        {
            SkillId = null,
            CustomUserAgent = "TestAgent/1.0"
        };
    }

    private static SkillServiceConfiguration CreateWhitespaceSkillIdConfiguration()
    {
        return new SkillServiceConfiguration
        {
            SkillId = "   ",
            CustomUserAgent = "TestAgent/1.0"
        };
    }

    private static SkillServiceConfiguration CreateDefaultConfiguration()
    {
        return new SkillServiceConfiguration
        {
            SkillId = "amzn1.ask.skill.default-test-id",
            CustomUserAgent = "TestAgent/1.0"
        };
    }
}