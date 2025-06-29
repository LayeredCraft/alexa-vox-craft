using System;
using System.Collections.Generic;
using System.Text.Json;
using AlexaVoxCraft.Model.Apl.DataSources;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLDataSourceConverter : BasePolymorphicConverter<APLDataSource>
{
    protected override IDictionary<string, Type> DerivedTypes => new Dictionary<string, Type>
    {
        { DynamicIndexList.DataSourceType, typeof(DynamicIndexList) },
        { ObjectDataSource.DataSourceType, typeof(ObjectDataSource) },
        { ListDataSource.DataSourceType, typeof(ListDataSource) },
        { DynamicTokenList.DataSourceType, typeof(DynamicTokenList) }
    };

    protected override Type? DefaultType => typeof(KeyValueDataSource);
    protected override Func<JsonElement, string?> KeyResolver => element =>
    {
        var typeValue = element.TryGetProperty(TypeDiscriminatorPropertyName, out var typeProp)
            ? typeProp.GetString()
            : nameof(KeyValueDataSource); // this will allow the default type to be used
        return typeValue;
    };
}