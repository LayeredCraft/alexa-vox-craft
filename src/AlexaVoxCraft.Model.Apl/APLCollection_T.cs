using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[CollectionBuilder(typeof(APLCollectionBuilder), nameof(APLCollectionBuilder.Create))]
[JsonConverter(typeof(APLCollectionConverterFactory))]
public class APLCollection<T> : APLValue, IEnumerable<T>
{
    private List<T> _items;

    public APLCollection()
    {
        _items = new List<T>();
    }

    public APLCollection(ReadOnlySpan<T> items)
    {
        _items = new List<T>(items.ToArray());
    }

    public APLCollection(IEnumerable<T> items)
    {
        _items = new List<T>(items);
    }

    public IList<T>? Items
    {
        get => _items;
        set => _items = value is null ? new List<T>() : new List<T>(value);
    }

    public override object? GetValue() => _items;

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Can't have implicit conversion from IEnumerable<T> (interface)
    // Use concrete types instead
    public static implicit operator APLCollection<T>?(List<T>? items)
    {
        return items is null ? null : new APLCollection<T>(items);
    }

    public static implicit operator APLCollection<T>?(T[]? items)
    {
        return items is null ? null : new APLCollection<T>(items);
    }

    public static implicit operator APLCollection<T>?(string? expression)
    {
        if (string.IsNullOrEmpty(expression))
            return null;

        return new APLCollection<T> { Expression = expression };
    }
}

public static class APLCollectionBuilder
{
    public static APLCollection<T> Create<T>(ReadOnlySpan<T> items)
    {
        return new APLCollection<T>(items);
    }
}