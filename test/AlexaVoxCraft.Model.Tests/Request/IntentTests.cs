using System.Text.Json;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.Tests.Request;

/// <summary>
/// Component-level coverage for <see cref="Intent"/> in isolation, independent of the request
/// envelope it normally arrives in.
/// </summary>
public sealed class IntentTests() : TestBase<IntentTests>
{
    [Fact]
    public async Task Intent_WithSlotResolution_Deserializes()
    {
        var json = Fx("Components/Intent_WithSlotResolution.json");
        var intent = JsonSerializer.Deserialize<Intent>(json, AlexaJson);

        intent.Should().NotBeNull();
        intent!.Name.Should().Be("CategorySelectionIntent");
        intent.Slots.Should().ContainKey("productCategory");
        intent.Slots["productCategory"].Resolution!.Authorities.Should().ContainSingle()
            .Which.Values.Should().ContainSingle()
            .Which.Value.Name.Should().Be("general");

        await TestHelper.VerifyRequestObject(intent);
    }

    [Fact]
    public void Signature_SimpleName_ParsesNamespaceAndAction()
    {
        var intent = new Intent { Name = "GetZodiacHoroscopeIntent" };

        intent.Signature.Action.Should().Be("GetZodiacHoroscopeIntent");
        intent.Signature.FullName.Should().Be("GetZodiacHoroscopeIntent");
    }

    [Fact]
    public void Signature_BuiltInIntentWithProperties_ParsesNamespaceActionAndProperties()
    {
        var intent = new Intent { Name = "AMAZON.AddAction<object@Book,targetCollection@ReadingList>" };

        intent.Signature.Namespace.Should().Be("AMAZON");
        intent.Signature.Action.Should().Be("AddAction");
        intent.Signature.Properties.Should().HaveCount(2);
        intent.Signature.Properties["object"].Entity.Should().Be("Book");
        intent.Signature.Properties["object"].Property.Should().BeNullOrEmpty();
        intent.Signature.Properties["targetCollection"].Entity.Should().Be("ReadingList");
    }

    [Fact]
    public async Task MultiValueSlot_Deserializes()
    {
        var json = Fx("Components/MultiValueSlot.json");
        var slots = JsonSerializer.Deserialize<Dictionary<string, Slot>>(json, AlexaJson);

        slots.Should().ContainSingle();
        var toppings = slots!["toppings"];
        toppings.SlotValue!.Values.Should().HaveCount(2);
        toppings.SlotValue.Values![0].Value.Should().Be("olives");
        toppings.SlotValue.Values[0].Resolutions!.Authorities.Should().ContainSingle()
            .Which.Values!.Should().HaveCount(2);

        await TestHelper.VerifyRequestObject(slots);
    }
}
