using System;
using System.Collections.Generic;
using AlexaVoxCraft.Model.Apl.Operation;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class OperationConverter : BasePolymorphicConverter<Operation.Operation>
{
    protected override IDictionary<string, Type> DerivedTypes => new Dictionary<string, Type>
    {
        [InsertItem.OperationType] = typeof(InsertItem),
        [InsertMultipleItems.OperationType] = typeof(InsertMultipleItems),
        [SetItem.OperationType] = typeof(SetItem),
        [DeleteItem.OperationType] = typeof(DeleteItem),
        [DeleteMultipleItems.OperationType] = typeof(DeleteMultipleItems)
    };
}