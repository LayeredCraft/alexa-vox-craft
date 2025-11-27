using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.Extensions;

namespace AlexaVoxCraft.Model.Tests.Responses;

public sealed class CardTests
{
    [Theory]
    [ModelAutoData]
    public void SimpleCard_WithGeneratedData_SerializesCorrectly(SimpleCard card)
    {
        card.Title.Should().NotBeNullOrEmpty();
        card.Content.Should().NotBeNullOrEmpty();
        
        card.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void StandardCard_WithGeneratedData_SerializesCorrectly(StandardCard card)
    {
        card.Title.Should().NotBeNullOrEmpty();
        card.Content.Should().NotBeNullOrEmpty();
        card.Image.Should().NotBeNull();
        card.Image.SmallImageUrl.Should().NotBeNullOrEmpty();
        card.Image.LargeImageUrl.Should().NotBeNullOrEmpty();
        
        card.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void AskForPermissionsConsentCard_WithGeneratedData_SerializesCorrectly(AskForPermissionsConsentCard card)
    {
        card.Permissions.Should().NotBeEmpty();
        card.Permissions.Should().OnlyContain(p => !string.IsNullOrEmpty(p));
        
        card.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void LinkAccountCard_WithGeneratedData_SerializesCorrectly(LinkAccountCard card)
    {
        card.Type.Should().Be("LinkAccount");
        
        card.ShouldRoundTripSerialize();
    }
}