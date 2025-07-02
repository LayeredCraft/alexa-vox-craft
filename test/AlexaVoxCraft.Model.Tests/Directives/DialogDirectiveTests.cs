using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.TestKit.Attributes;
using AlexaVoxCraft.TestKit.Extensions;
using AwesomeAssertions;

namespace AlexaVoxCraft.Model.Tests.Directives;

public sealed class DialogDirectiveTests
{
    [Theory]
    [ModelAutoData]
    public void DialogDelegate_WithGeneratedData_SerializesCorrectly(DialogDelegate directive)
    {
        directive.Type.Should().Be("Dialog.Delegate");
        directive.UpdatedIntent.Should().NotBeNull();
        directive.UpdatedIntent.Name.Should().NotBeNullOrEmpty();
        directive.UpdatedIntent.Slots.Should().NotBeEmpty();
        
        directive.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void DialogElicitSlot_WithGeneratedData_SerializesCorrectly(DialogElicitSlot directive)
    {
        directive.Type.Should().Be("Dialog.ElicitSlot");
        directive.SlotName.Should().NotBeNullOrEmpty();
        directive.UpdatedIntent.Should().NotBeNull();
        directive.UpdatedIntent.Name.Should().NotBeNullOrEmpty();
        
        directive.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void DialogConfirmSlot_WithGeneratedData_SerializesCorrectly(DialogConfirmSlot directive)
    {
        directive.Type.Should().Be("Dialog.ConfirmSlot");
        directive.SlotName.Should().NotBeNullOrEmpty();
        directive.UpdatedIntent.Should().NotBeNull();
        directive.UpdatedIntent.Name.Should().NotBeNullOrEmpty();
        
        directive.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void DialogConfirmIntent_WithGeneratedData_SerializesCorrectly(DialogConfirmIntent directive)
    {
        directive.Type.Should().Be("Dialog.ConfirmIntent");
        directive.UpdatedIntent.Should().NotBeNull();
        directive.UpdatedIntent.Name.Should().NotBeNullOrEmpty();
        directive.UpdatedIntent.Slots.Should().NotBeEmpty();
        
        directive.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void DialogUpdateDynamicEntities_WithGeneratedData_SerializesCorrectly(DialogUpdateDynamicEntities directive)
    {
        directive.Type.Should().Be("Dialog.UpdateDynamicEntities");
        directive.UpdateBehavior.Should().BeOneOf(UpdateBehavior.Replace, UpdateBehavior.Clear);
        directive.Types.Should().NotBeEmpty();
        
        foreach (var slotType in directive.Types)
        {
            slotType.Name.Should().NotBeNullOrEmpty();
            slotType.Values.Should().NotBeEmpty();
            
            foreach (var value in slotType.Values)
            {
                value.Id.Should().NotBeNullOrEmpty();
                value.Name.Should().NotBeNull();
                value.Name.Value.Should().NotBeNullOrEmpty();
            }
        }
        
        directive.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void Intent_WithGeneratedData_HasValidStructure(Intent intent)
    {
        intent.Name.Should().NotBeNullOrEmpty();
        intent.ConfirmationStatus.Should().BeOneOf(
            ConfirmationStatus.None, 
            ConfirmationStatus.Confirmed, 
            ConfirmationStatus.Denied);
        intent.Slots.Should().NotBeEmpty();
        
        foreach (var slot in intent.Slots.Values)
        {
            slot.Name.Should().NotBeNullOrEmpty();
            slot.Value.Should().NotBeNullOrEmpty();
            slot.ConfirmationStatus.Should().BeOneOf(
                ConfirmationStatus.None, 
                ConfirmationStatus.Confirmed, 
                ConfirmationStatus.Denied);
        }
        
        intent.ShouldRoundTripSerialize();
    }

    [Theory]
    [ModelAutoData]
    public void SlotType_WithGeneratedData_HasValidStructure(SlotType slotType)
    {
        slotType.Name.Should().NotBeNullOrEmpty();
        slotType.Values.Should().NotBeEmpty();
        
        foreach (var value in slotType.Values)
        {
            value.Id.Should().NotBeNullOrEmpty();
            value.Name.Should().NotBeNull();
            value.Name.Value.Should().NotBeNullOrEmpty();
            value.Name.Synonyms.Should().NotBeNull();
        }
        
        slotType.ShouldRoundTripSerialize();
    }
}