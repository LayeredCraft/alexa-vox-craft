using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

/// <summary>
/// Represents an APL value that serializes as either:
/// - a JSON array (when item-backed), or
/// - a raw expression string (when expression-backed).
/// Mutations materialize the collection by clearing any existing expression.
/// </summary>
[CollectionBuilder(typeof(APLValueCollectionBuilder), nameof(APLValueCollectionBuilder.Create))]
[JsonConverter(typeof(APLValueCollectionConverterFactory))]
public sealed class APLValueCollection<T> : APLValue, IList<T>, IReadOnlyList<T>
{
    private readonly List<T> _items;

    /// <summary>
    /// Initializes an empty, item-backed collection.
    /// </summary>
    public APLValueCollection()
    {
        _items = [];
    }

    /// <summary>
    /// Initializes an item-backed collection.
    /// </summary>
    public APLValueCollection(ReadOnlySpan<T> items)
    {
        _items = [..items];
    }

    /// <summary>
    /// Initializes an item-backed collection.
    /// </summary>
    public APLValueCollection(IEnumerable<T> items)
    {
        _items = [..items];
    }

    /// <summary>
    /// A read-only view of the current items (helpful for debugging/inspection).
    /// Prefer list APIs directly on this type (Count/Add/Remove/etc.).
    /// </summary>
    public IReadOnlyList<T> Items => _items;

    /// <inheritdoc />
    public override object GetValue() => _items;

    /// <inheritdoc />
    public int Count => _items.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public T this[int index]
    {
        get => _items[index];
        set
        {
            Materialize();
            _items[index] = value;
        }
    }

    /// <inheritdoc />
    public void Add(T item)
    {
        Materialize();
        _items.Add(item);
    }

    /// <inheritdoc />
    public void Clear()
    {
        Materialize();
        _items.Clear();
    }

    /// <inheritdoc />
    public bool Contains(T item) => _items.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public int IndexOf(T item) => _items.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
        Materialize();
        _items.Insert(index, item);
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        Materialize();
        return _items.Remove(item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        Materialize();
        _items.RemoveAt(index);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void Materialize()
    {
        if (!string.IsNullOrEmpty(Expression))
        {
            Expression = null;
        }
    }

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