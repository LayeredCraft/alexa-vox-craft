using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.TestKit.Attributes;
using AlexaVoxCraft.TestKit.Extensions;
using AwesomeAssertions;

namespace AlexaVoxCraft.Model.Tests.Responses;

public sealed class CardTests
{
    [Theory]
    [ModelAutoData]
    public void SimpleCard_WithGeneratedData_SerializesCorrectly(SimpleCard card)
    {
        // Verify generated data meets Alexa constraints
        card.Title.Should().NotBeNullOrEmpty();
        card.Content.Should().NotBeNullOrEmpty();
        
        // Verify serialization works correctly
        card.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void StandardCard_WithGeneratedData_SerializesCorrectly(StandardCard card)
    {
        // Verify generated data meets Alexa constraints
        card.Title.Should().NotBeNullOrEmpty();
        card.Content.Should().NotBeNullOrEmpty();
        card.Image.Should().NotBeNull();
        card.Image.SmallImageUrl.Should().NotBeNullOrEmpty();
        card.Image.LargeImageUrl.Should().NotBeNullOrEmpty();
        
        // Verify serialization works correctly
        card.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void AskForPermissionsConsentCard_WithGeneratedData_SerializesCorrectly(AskForPermissionsConsentCard card)
    {
        // Verify generated data meets Alexa constraints
        card.Permissions.Should().NotBeEmpty();
        card.Permissions.Should().OnlyContain(p => !string.IsNullOrEmpty(p));
        
        // Verify serialization works correctly
        card.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void LinkAccountCard_WithGeneratedData_SerializesCorrectly(LinkAccountCard card)
    {
        // Verify card type is correct
        card.Type.Should().Be("LinkAccount");
        
        // Verify serialization works correctly
        card.ShouldRoundTripSerialize();
    }
}