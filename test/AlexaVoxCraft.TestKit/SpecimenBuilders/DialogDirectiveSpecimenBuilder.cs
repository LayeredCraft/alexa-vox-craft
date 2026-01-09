using AlexaVoxCraft.Model;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response.Directive;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public sealed class DialogDirectiveSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type)
        {
            if (type == typeof(DialogDelegate))
                return CreateDialogDelegate(context);
            
            if (type == typeof(DialogElicitSlot))
                return CreateDialogElicitSlot(context);
            
            if (type == typeof(DialogConfirmSlot))
                return CreateDialogConfirmSlot(context);
            
            if (type == typeof(DialogConfirmIntent))
                return CreateDialogConfirmIntent(context);
            
            if (type == typeof(DialogUpdateDynamicEntities))
                return CreateDialogUpdateDynamicEntities(context);
            
            if (type == typeof(Intent))
                return CreateIntent(context);
            
            if (type == typeof(Slot))
                return CreateSlot(context);
            
            if (type == typeof(SlotType))
                return CreateSlotType(context);
            
            if (type == typeof(SlotTypeValue))
                return CreateSlotTypeValue(context);
            
            if (type == typeof(SlotTypeValueName))
                return CreateSlotTypeValueName(context);
        }
        
        return new NoSpecimen();
    }
    
    private static DialogDelegate CreateDialogDelegate(ISpecimenContext context)
    {
        return new DialogDelegate
        {
            UpdatedIntent = CreateIntent(context)
        };
    }
    
    private static DialogElicitSlot CreateDialogElicitSlot(ISpecimenContext context)
    {
        var slotNames = new[] { "ZodiacSign", "Date", "Time", "Location", "Number" };
        var slotName = slotNames[context.Create<int>() % slotNames.Length];
        
        return new DialogElicitSlot(slotName)
        {
            UpdatedIntent = CreateIntent(context)
        };
    }
    
    private static DialogConfirmSlot CreateDialogConfirmSlot(ISpecimenContext context)
    {
        var slotNames = new[] { "Date", "Time", "Location", "Amount", "Confirmation" };
        var slotName = slotNames[context.Create<int>() % slotNames.Length];
        
        return new DialogConfirmSlot(slotName)
        {
            UpdatedIntent = CreateIntent(context)
        };
    }
    
    private static DialogConfirmIntent CreateDialogConfirmIntent(ISpecimenContext context)
    {
        return new DialogConfirmIntent
        {
            UpdatedIntent = CreateIntent(context)
        };
    }
    
    private static DialogUpdateDynamicEntities CreateDialogUpdateDynamicEntities(ISpecimenContext context)
    {
        var updateBehaviors = new[] { UpdateBehavior.Replace, UpdateBehavior.Clear };
        var updateBehavior = updateBehaviors[context.Create<int>() % updateBehaviors.Length];
        
        var directive = new DialogUpdateDynamicEntities
        {
            UpdateBehavior = updateBehavior
        };
        
        var typeCount = context.Create<int>() % 3 + 1; // 1-3 slot types
        for (int i = 0; i < typeCount; i++)
        {
            directive.Types.Add(CreateSlotType(context));
        }
        
        return directive;
    }
    
    private static Intent CreateIntent(ISpecimenContext context)
    {
        var intentNames = new[]
        {
            "GetZodiacHoroscopeIntent", "BookFlightIntent", "OrderPizzaIntent",
            "WeatherIntent", "PlayMusicIntent"
        };
        
        var intentName = intentNames[context.Create<int>() % intentNames.Length];
        var confirmationStatuses = new[]
        {
            ConfirmationStatus.None, ConfirmationStatus.Confirmed, ConfirmationStatus.Denied
        };
        var confirmationStatus = confirmationStatuses[context.Create<int>() % confirmationStatuses.Length];
        
        var intent = new Intent
        {
            Name = intentName,
            ConfirmationStatus = confirmationStatus,
            Slots = new Dictionary<string, Slot>()
        };
        
        var slotCount = context.Create<int>() % 3 + 1; // 1-3 slots
        var slotNames = new[] { "ZodiacSign", "Date", "Time", "Location", "Number" };
        
        for (int i = 0; i < slotCount && i < slotNames.Length; i++)
        {
            intent.Slots[slotNames[i]] = CreateSlot(context, slotNames[i]);
        }
        
        return intent;
    }
    
    private static Slot CreateSlot(ISpecimenContext context, string slotName = null)
    {
        var slotNames = new[] { "ZodiacSign", "Date", "Time", "Location", "Number" };
        var name = slotName ?? slotNames[context.Create<int>() % slotNames.Length];
        
        var values = new Dictionary<string, string[]>
        {
            { "ZodiacSign", new[] { "virgo", "leo", "aries", "gemini" } },
            { "Date", new[] { "2015-11-25", "2024-01-15", "2023-12-31" } },
            { "Time", new[] { "14:30", "09:00", "18:45" } },
            { "Location", new[] { "Seattle", "Boston", "New York" } },
            { "Number", new[] { "5", "10", "25" } }
        };
        
        var value = values.ContainsKey(name) 
            ? values[name][context.Create<int>() % values[name].Length]
            : $"value_{context.Create<int>() % 100}";
        
        var confirmationStatuses = new[]
        {
            ConfirmationStatus.None, ConfirmationStatus.Confirmed, ConfirmationStatus.Denied
        };
        var confirmationStatus = confirmationStatuses[context.Create<int>() % confirmationStatuses.Length];
        
        return new Slot
        {
            Name = name,
            Value = value,
            ConfirmationStatus = confirmationStatus
        };
    }
    
    private static SlotType CreateSlotType(ISpecimenContext context)
    {
        var typeNames = new[] { "AirportSlotType", "CitySlotType", "FoodSlotType" };
        var typeName = typeNames[context.Create<int>() % typeNames.Length];
        
        var slotType = new SlotType
        {
            Name = typeName,
            Values = new SlotTypeValue[0]
        };
        
        var valueCount = context.Create<int>() % 3 + 1; // 1-3 values
        var values = new List<SlotTypeValue>();
        
        for (int i = 0; i < valueCount; i++)
        {
            values.Add(CreateSlotTypeValue(context));
        }
        
        slotType.Values = values.ToArray();
        return slotType;
    }
    
    private static SlotTypeValue CreateSlotTypeValue(ISpecimenContext context)
    {
        var ids = new[] { "BOS", "LGA", "JFK", "LAX", "SEA" };
        var id = ids[context.Create<int>() % ids.Length];
        
        return new SlotTypeValue
        {
            Id = id,
            Name = CreateSlotTypeValueName(context)
        };
    }
    
    private static SlotTypeValueName CreateSlotTypeValueName(ISpecimenContext context)
    {
        var values = new[]
        {
            "Logan International Airport", "LaGuardia Airport", "JFK Airport",
            "Los Angeles International", "Seattle-Tacoma International"
        };
        var value = values[context.Create<int>() % values.Length];
        
        var synonyms = new[]
        {
            new[] { "Boston Logan" },
            new[] { "New York" },
            new[] { "Kennedy Airport" },
            new[] { "LAX" },
            new[] { "Sea-Tac" }
        };
        var synonym = synonyms[context.Create<int>() % synonyms.Length];
        
        return new SlotTypeValueName
        {
            Value = value,
            Synonyms = synonym
        };
    }
}