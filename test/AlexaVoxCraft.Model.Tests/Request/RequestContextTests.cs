using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Tests.Request;

/// <summary>
/// Component- and context-level coverage for request context data not exercised by the trivia
/// skill's captured payloads: Geolocation, Person, and SmartProperties (unit/persistent endpoint id).
/// </summary>
public sealed class RequestContextTests() : TestBase<RequestContextTests>
{
    [Fact]
    public async Task Geolocation_Deserializes()
    {
        var json = Fx("Components/Geolocation.json");
        var location = JsonSerializer.Deserialize<Geolocation>(json, AlexaJson);

        location.Should().NotBeNull();
        location!.LocationServices!.Access.Should().Be(LocationServiceAccess.Enabled);
        location.LocationServices.Status.Should().Be(LocationServiceStatus.Running);
        location.Coordinate!.Latitude.Should().Be(47.6);
        location.Coordinate.Longitude.Should().Be(-122.3);
        location.Altitude!.Altitude.Should().Be(56.4);
        location.Heading!.Direction.Should().Be(180.0);
        location.Speed!.Speed.Should().Be(10.0);

        await TestHelper.VerifyRequestObject(location);
    }

    [Fact]
    public async Task IntentRequest_WithPerson_Deserializes()
    {
        var json = Fx("Requests/IntentRequest_WithPerson.json");
        var envelope = JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        envelope!.Context.System.Person.Should().NotBeNull();
        envelope.Context.System.Person!.PersonId.Should().Be("amzn1.ask.account.personid");
        envelope.Context.System.Person.AccessToken.Should().Be("Atza|BBBBBBB");
        envelope.Context.System.Person.AuthenticationConfidenceLevel!.Level.Should().Be(300);

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task IntentRequest_WithSmartProperties_Deserializes()
    {
        var json = Fx("Requests/IntentRequest_SmartProperties.json");
        var envelope = JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        envelope!.Context.System.Unit.Should().NotBeNull();
        envelope.Context.System.Unit!.UnitID.Should().Be("amzn1.ask.unit.A0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        envelope.Context.System.Unit.PersistentUnitID.Should().Be("amzn1.alexa.unit.did.X7Y8Z9");
        envelope.Context.System.Device!.PersistentEndpointID.Should().Be("amzn1.alexa.endpoint.AABBCC010101010101010101");

        await TestHelper.VerifyRequestObject(envelope);
    }
}
