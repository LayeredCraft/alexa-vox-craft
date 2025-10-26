namespace AlexaVoxCraft.MediatR.Generator.Tests.TestKit.Attributes;

public sealed class GeneratorAutoDataAttribute() : AutoDataAttribute(CreateFixture)
{
    internal static IFixture CreateFixture()
    {
        return BaseFixtureFactory.CreateFixture(fixture => { });
    }
}

public sealed class InlineGeneratorAutoDataAttribute(params object[] values)
    : InlineAutoDataAttribute(GeneratorAutoDataAttribute.CreateFixture, values);