namespace AlexaVoxCraft.TestKit.Attributes;

public class AlexaVoxCraftAutoDataAttribute() : AutoDataAttribute(CreateFixture)
{
    internal static IFixture CreateFixture()
    {
        return BaseFixtureFactory.CreateFixture(fixture =>
        {
        });
    }
}

public class InlineAlexaVoxCraftAutoDataAttribute(params object[] values)
    : InlineAutoDataAttribute(AlexaVoxCraftAutoDataAttribute.CreateFixture, values)
{
}