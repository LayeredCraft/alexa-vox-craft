using System.Reflection;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.Model.Response.Ssml;
using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public class OptionsSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public OptionsSpecimenBuilder() : this(new OptionsSpecification())
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
            _ when parameterName.Contains("nullconfigurationvalue") || parameterName.Contains("nullvalue") => CreateNullValueOptions(),
            _ when parameterName.Contains("invalidconfiguration") => CreateInvalidOptions(),
            _ when parameterName.Contains("validconfiguration") => CreateValidOptions(),
            _ when parameterName.Contains("emptyconfiguration") => CreateEmptyOptions(),
            _ when parameterName.Contains("whitespaceconfiguration") => CreateWhitespaceOptions(),
            _ when parameterName.Contains("voicedconfiguration") || parameterName.Contains("withvoice") => CreateOptionsWithDefaultVoice(),
            _ => CreateDefaultOptions()
        };
    }

    private static IOptions<SkillServiceConfiguration> CreateNullValueOptions()
    {
        var options = Substitute.For<IOptions<SkillServiceConfiguration>>();
        options.Value.Returns((SkillServiceConfiguration)null!);
        return options;
    }

    private static IOptions<SkillServiceConfiguration> CreateInvalidOptions()
    {
        var config = new SkillServiceConfiguration
        {
            SkillId = "different-skill-id",
            CustomUserAgent = "TestAgent/1.0"
        };
        return Options.Create(config);
    }

    private static IOptions<SkillServiceConfiguration> CreateValidOptions()
    {
        var config = new SkillServiceConfiguration
        {
            SkillId = "amzn1.ask.skill.test-skill-id",
            CustomUserAgent = "TestAgent/1.0"
        };
        return Options.Create(config);
    }

    private static IOptions<SkillServiceConfiguration> CreateEmptyOptions()
    {
        var config = new SkillServiceConfiguration
        {
            SkillId = "",
            CustomUserAgent = "TestAgent/1.0"
        };
        return Options.Create(config);
    }

    private static IOptions<SkillServiceConfiguration> CreateWhitespaceOptions()
    {
        var config = new SkillServiceConfiguration
        {
            SkillId = "   ",
            CustomUserAgent = "TestAgent/1.0"
        };
        return Options.Create(config);
    }

    private static IOptions<SkillServiceConfiguration> CreateDefaultOptions()
    {
        var config = new SkillServiceConfiguration
        {
            SkillId = "amzn1.ask.skill.default-test-id",
            CustomUserAgent = "TestAgent/1.0"
        };
        return Options.Create(config);
    }

    private static IOptions<SkillServiceConfiguration> CreateOptionsWithDefaultVoice()
    {
        var config = new SkillServiceConfiguration
        {
            SkillId = "amzn1.ask.skill.default-test-id",
            CustomUserAgent = "TestAgent/1.0",
            DefaultVoiceName = AlexaSupportedVoices.EnglishUS.Matthew
        };
        return Options.Create(config);
    }
}