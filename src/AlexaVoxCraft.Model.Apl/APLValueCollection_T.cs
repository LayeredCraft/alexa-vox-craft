using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[CollectionBuilder(typeof(APLValueCollectionBuilder), nameof(APLValueCollectionBuilder.Create))]
[JsonConverter(typeof(APLValueCollectionConverterFactory))]
public class APLValueCollection<T> : APLValue, IEnumerable<T>
{
    private List<T> _items;

    public APLValueCollection()
    {
        _items = [];
    }

    public APLValueCollection(ReadOnlySpan<T> items)
    {
        _items = [..items];
    }

    public APLValueCollection(IEnumerable<T> items)
    {
        _items = [..items];
    }

    public IList<T>? Items
    {
        get => _items;
        set => _items = value is null ? [] : [..value];
    }

    public override object GetValue() => _items;

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Can't have implicit conversion from IEnumerable<T> (interface)
    // Use concrete types instead
    public static implicit operator APLValueCollection<T>?(List<T>? items)
    {
        return items is null ? null : new APLValueCollection<T>(items);
    }

    public static implicit operator APLValueCollection<T>?(T[]? items)
    {
        return items is null ? null : new APLValueCollection<T>(items);
    }

    public static implicit operator APLValueCollection<T>?(string? expression)
    {
        return string.IsNullOrEmpty(expression) ? null : new APLValueCollection<T> { Expression = expression };
    }
}

public static class APLValueCollectionBuilder
{
    public static APLValueCollection<T> Create<T>(ReadOnlySpan<T> items)
    {
        return new APLValueCollection<T>(items);
    }
}