using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.Extensions;
using Compono.XunitV3;
using AlexaVoxCraft.Model.Tests.TestKit;

namespace AlexaVoxCraft.Model.Tests.Responses;

public sealed class CardTests
{
    [Theory, Compose<ModelLegacyProfile>]
    public void SimpleCard_WithGeneratedData_SerializesCorrectly(SimpleCard card)
    {
        card.Title.Should().NotBeNullOrEmpty();
        card.Content.Should().NotBeNullOrEmpty();

        card.ShouldRoundTripSerialize();
    }

    [Theory, Compose<ModelLegacyProfile>]
    public void StandardCard_WithGeneratedData_SerializesCorrectly(StandardCard card)
    {
        card.Title.Should().NotBeNullOrEmpty();
        card.Content.Should().NotBeNullOrEmpty();
        card.Image.Should().NotBeNull();
        card.Image.SmallImageUrl.Should().NotBeNullOrEmpty();
        card.Image.LargeImageUrl.Should().NotBeNullOrEmpty();

        card.ShouldRoundTripSerialize();
    }

    [Theory, Compose<ModelLegacyProfile>]
    public void AskForPermissionsConsentCard_WithGeneratedData_SerializesCorrectly(AskForPermissionsConsentCard card)
    {
        card.Permissions.Should().NotBeEmpty();
        card.Permissions.Should().OnlyContain(p => !string.IsNullOrEmpty(p));

        card.ShouldRoundTripSerialize();
    }

    [Theory, Compose<ModelLegacyProfile>]
    public void LinkAccountCard_WithGeneratedData_SerializesCorrectly(LinkAccountCard card)
    {
        card.Type.Should().Be("LinkAccount");

        card.ShouldRoundTripSerialize();
    }
}
