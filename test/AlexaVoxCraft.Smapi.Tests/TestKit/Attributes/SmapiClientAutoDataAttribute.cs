using AlexaVoxCraft.Http.TestKit.Attributes;
using AlexaVoxCraft.Smapi.Tests.TestKit.SpecimenBuilders;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.Attributes;

/// <summary>
/// Provides auto-generated test data with configured HttpClient and HttpMessageHandler for client testing.
/// </summary>
public sealed class SmapiClientAutoDataAttribute() : ClientAutoDataAttribute(CreateFixture)
{
    internal static IFixture CreateFixture()
    {
        return CreateBaseFixture(fixture =>
        {
            fixture.Customizations.Add(new InteractionModelDefinitionSpecimenBuilder());
        });
    }
}

/// <summary>
/// Provides inline test data combined with auto-generated client test data.
/// </summary>
public sealed class InlineSmapiClientAutoDataAttribute(params object?[] values)
    : InlineAutoDataAttribute(SmapiClientAutoDataAttribute.CreateFixture, values);