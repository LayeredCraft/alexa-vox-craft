using System.Reflection;
using AlexaVoxCraft.Smapi.Models.InteractionModel;
using AlexaVoxCraft.Smapi.Tests.TestKit.RequestSpecifications;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.SpecimenBuilders;

/// <summary>
/// AutoFixture specimen builder that creates realistic InteractionModelDefinition instances with complex nested structures.
/// </summary>
public sealed class InteractionModelDefinitionSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionModelDefinitionSpecimenBuilder"/> class
    /// with the default InteractionModelDefinitionSpecification.
    /// </summary>
    public InteractionModelDefinitionSpecimenBuilder() : this(new InteractionModelDefinitionSpecification())
    {

    }

    /// <summary>
    /// Creates a complex InteractionModelDefinition with intents, slots, and custom slot types.
    /// </summary>
    /// <param name="request">The specimen request.</param>
    /// <param name="context">The specimen context.</param>
    /// <returns>A configured InteractionModelDefinition instance, or NoSpecimen if the request doesn't match.</returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        var parameterName = request switch
        {
            ParameterInfo parameter => parameter.Name?.ToLowerInvariant() ?? "",
            Type type => "",
            _ => throw new ArgumentException("Invalid request type", nameof(request))
        };

        // Future: Use parameterName to create different model variations
        // e.g., if (parameterName.Contains("complex")) { ... }

        var version = context.Create<string>();
        var description = context.Create<string>();
        var invocationName = context.Create<string>().ToLowerInvariant();

        var slotTypeName = context.Create<string>();
        var slotTypeValue = context.Create<string>();
        var synonym1 = context.Create<string>();
        var synonym2 = context.Create<string>();

        var customSlotType = new SlotType
        {
            Name = slotTypeName,
            Values =
            [
                new SlotTypeValue
                {
                    Name = new SlotTypeValueName
                    {
                        Value = slotTypeValue,
                        Synonyms = [synonym1, synonym2]
                    }
                }
            ]
        };

        var intentSlot = new IntentSlot
        {
            Name = "testSlot",
            Type = slotTypeName,
            IsRequired = true
        };

        var intent = new Intent
        {
            Name = context.Create<string>(),
            Samples = [context.Create<string>(), context.Create<string>()],
            Slots = [intentSlot]
        };

        var languageModel = new LanguageModel
        {
            InvocationName = invocationName,
            Intents = [intent],
            Types = [customSlotType]
        };

        var interactionModelBody = new InteractionModelBody
        {
            LanguageModel = languageModel
        };

        return new InteractionModelDefinition
        {
            Version = version,
            Description = description,
            InteractionModel = interactionModelBody
        };
    }
}