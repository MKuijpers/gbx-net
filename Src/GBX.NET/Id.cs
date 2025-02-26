﻿namespace GBX.NET;

/// <summary>
/// A struct that handles Id (lookback string) by either a string or an index.
/// </summary>
public readonly record struct Id
{
    /// <summary>
    /// Represents an ID of the collection. Null if the <see cref="Id"/> is string-defined.
    /// </summary>
    public int? Index { get; }

    /// <summary>
    /// Represents the string of the <see cref="Id"/>. Null if the <see cref="Id"/> is presented as a collection ID.
    /// </summary>
    public string? String { get; }

    /// <summary>
    /// Constructs an <see cref="Id"/> struct from a string representation.
    /// </summary>
    /// <param name="str">An Id string.</param>
    public Id(string str)
    {
        Index = null;
        String = str;
    }

    /// <summary>
    /// Constructs an <see cref="Id"/> struct from an ID reprentation.
    /// </summary>
    /// <param name="collectionId">A collection ID from the <see cref="Resources.CollectionID"/> list (specified ID doesn't have to be available in the list).</param>
    public Id(int collectionId)
    {
        Index = collectionId;
        String = null;
    }

    public Int3 GetBlockSize() => ToString() switch
    {
        "Desert" => (32, 16, 32),
        "Snow" => (32, 16, 32),
        "Rally" => (32, 16, 32),
        "Island" => (64, 8, 64),
        "Bay" => (32, 8, 32),
        "Coast" => (16, 8, 16),
        "Valley" => (32, 8, 32),
        "Stadium" => (32, 8, 32),
        "Canyon" => (64, 16, 64),
        "Lagoon" => (32, 8, 32),
        _ => throw new Exception(),
    };

    /// <summary>
    /// Converts the <see cref="Id"/> to string, also into its readable form if the <see cref="Id"/> is presented by collection ID.
    /// </summary>
    /// <returns>If collection is ID-represented, the ID is converted to <see cref="string"/> based from the <see cref="Resources.CollectionID"/> list. If it's string-represented, <see cref="String"/> is returned instead.</returns>
    public override string ToString()
    {
        if (Index is null)
        {
            return String ?? "";
        }

        if (NodeCacheManager.CollectionIds.TryGetValue(Index.Value, out string? value))
        {
            return value;
        }

        return Index.Value.ToString();
    }

    public static implicit operator Id(string a)
    {
        if (string.IsNullOrEmpty(a))
        {
            return new Id();
        }

        if (int.TryParse(a, out int collectionID))
        {
            return new Id(collectionID);
        }
        
        return new Id(a);
    }

    public static implicit operator Id(int a) => new(a);
    public static implicit operator string(Id a) => a.ToString();
}
