using AlexaVoxCraft.Http.TestKit.Attributes;
using AlexaVoxCraft.InSkillPurchasing.Tests.TestKit.SpecimenBuilders;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.TestKit.Attributes;

/// <summary>
/// Provides auto-generated test data with configured HttpClient and HttpMessageHandler for ISP client testing.
/// </summary>
public sealed class IspClientAutoDataAttribute() : ClientAutoDataAttribute(CreateFixture)
{
    internal static IFixture CreateFixture() => CreateBaseFixture(fixture =>
    {
        fixture.Customizations.Add(new InSkillProductSpecimenBuilder());
        fixture.Customizations.Add(new TransactionSpecimenBuilder());
    });
}

/// <summary>
/// Provides inline test data combined with auto-generated ISP client test data.
/// </summary>
public sealed class InlineIspClientAutoDataAttribute(params object?[] values)
    : InlineAutoDataAttribute(IspClientAutoDataAttribute.CreateFixture, values);