using AlexaVoxCraft.Smapi.Tests.TestKit.SpecimenBuilders;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.Attributes;

/// <summary>
/// Provides auto-generated test data with configured HttpClient and HttpMessageHandler for client testing.
/// </summary>
public sealed class ClientAutoDataAttribute() : AutoDataAttribute(CreateFixture)
{
    internal static IFixture CreateFixture()
    {
        return BaseFixtureFactory.CreateFixture(fixture =>
        {
            fixture.Freeze<HttpMessageHandler>();
            fixture.Customizations.Add(new HttpClientSpecimenBuilder());
            fixture.Customizations.Add(new InteractionModelDefinitionSpecimenBuilder());
        });
    }
}

/// <summary>
/// Provides inline test data combined with auto-generated client test data.
/// </summary>
public sealed class InlineClientAutoDataAttribute(params object[] values)
    : InlineAutoDataAttribute(ClientAutoDataAttribute.CreateFixture, values);