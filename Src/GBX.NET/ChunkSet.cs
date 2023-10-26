﻿using System.Diagnostics.CodeAnalysis;

namespace GBX.NET;

public class ChunkSet : SortedSet<Chunk>
{
    public ChunkSet() : base()
    {
        
    }

    public ChunkSet(IEnumerable<Chunk> collection) : base(collection)
    {
        
    }

    public bool Remove(uint chunkID)
    {
        return RemoveWhere(x => x.Id == chunkID) > 0;
    }

    public bool Remove<T>() where T : Chunk
    {
        return RemoveWhere(x => x is T) > 0;
    }

    public Chunk Create(uint chunkId)
    {
        if (TryGet(chunkId, out var c))
        {
            return c ?? throw new ThisShouldNotHappenException();
        }

        var chunk = NodeManager.GetNewChunk(chunkId) ?? throw new Exception("Chunk ID does not exist.");
        
        if (chunk is ISkippableChunk skippableChunk)
        {
            skippableChunk.Discovered = true;
        }

        Add(chunk);

        return chunk;
    }

    public T Create<T>() where T : Chunk
    {
        return (T)Create(NodeManager.ChunkIdsByType[typeof(T)]);
    }

    public Chunk? Get(uint chunkId)
    {
        return this.FirstOrDefault(x => x.Id == chunkId);
    }

#if NET462_OR_GREATER || NETSTANDARD2_0
    public bool TryGet(uint chunkId, out Chunk? chunk)
#else
    public bool TryGet(uint chunkId, [NotNullWhen(true)] out Chunk? chunk)
#endif
    {
        chunk = Get(chunkId);
        return chunk is not null;
    }

    public T? Get<T>() where T : Chunk
    {
        foreach (var chunk in this)
        {
            if (chunk is T t)
            {
                if (chunk is ISkippableChunk s) s.Discover();
                return t;
            }
        }
        return default;
    }

    public bool TryGet<T>(out T? chunk) where T : Chunk
    {
        chunk = Get<T>();
        return chunk is not null;
    }

    public ISkippableChunk? GetSkippable(uint chunkId)
    {
        return this.FirstOrDefault(x => x.Id == chunkId) as ISkippableChunk;
    }

    public bool TryGetSkippable(uint chunkId, out ISkippableChunk? chunk)
    {
        chunk = GetSkippable(chunkId);
        return chunk is not null;
    }

    public void Discover<TChunk1>() where TChunk1 : ISkippableChunk
    {
        foreach (var chunk in this)
            if (chunk is TChunk1 c)
                c.Discover();
    }

    public void Discover<TChunk1, TChunk2>() where TChunk1 : ISkippableChunk where TChunk2 : ISkippableChunk
    {
        foreach (var chunk in this)
        {
            if (chunk is TChunk1 c1) c1.Discover();
            if (chunk is TChunk2 c2) c2.Discover();
        }
    }

    public void Discover<TChunk1, TChunk2, TChunk3>()
        where TChunk1 : ISkippableChunk
        where TChunk2 : ISkippableChunk
        where TChunk3 : ISkippableChunk
    {
        foreach (var chunk in this)
        {
            if (chunk is TChunk1 c1) c1.Discover();
            if (chunk is TChunk2 c2) c2.Discover();
            if (chunk is TChunk3 c3) c3.Discover();
        }
    }

    public void Discover<TChunk1, TChunk2, TChunk3, TChunk4>()
        where TChunk1 : ISkippableChunk
        where TChunk2 : ISkippableChunk
        where TChunk3 : ISkippableChunk
        where TChunk4 : ISkippableChunk
    {
        foreach (var chunk in this)
        {
            if (chunk is TChunk1 c1) c1.Discover();
            if (chunk is TChunk2 c2) c2.Discover();
            if (chunk is TChunk3 c3) c3.Discover();
            if (chunk is TChunk4 c4) c4.Discover();
        }
    }

    public void Discover<TChunk1, TChunk2, TChunk3, TChunk4, TChunk5>()
        where TChunk1 : ISkippableChunk
        where TChunk2 : ISkippableChunk
        where TChunk3 : ISkippableChunk
        where TChunk4 : ISkippableChunk
        where TChunk5 : ISkippableChunk
    {
        foreach (var chunk in this)
        {
            if (chunk is TChunk1 c1) c1.Discover();
            if (chunk is TChunk2 c2) c2.Discover();
            if (chunk is TChunk3 c3) c3.Discover();
            if (chunk is TChunk4 c4) c4.Discover();
            if (chunk is TChunk5 c5) c5.Discover();
        }
    }

    public void Discover<TChunk1, TChunk2, TChunk3, TChunk4, TChunk5, TChunk6>()
        where TChunk1 : ISkippableChunk
        where TChunk2 : ISkippableChunk
        where TChunk3 : ISkippableChunk
        where TChunk4 : ISkippableChunk
        where TChunk5 : ISkippableChunk
        where TChunk6 : ISkippableChunk
    {
        foreach (var chunk in this)
        {
            if (chunk is TChunk1 c1) c1.Discover();
            if (chunk is TChunk2 c2) c2.Discover();
            if (chunk is TChunk3 c3) c3.Discover();
            if (chunk is TChunk4 c4) c4.Discover();
            if (chunk is TChunk5 c5) c5.Discover();
            if (chunk is TChunk6 c6) c6.Discover();
        }
    }

    /// <summary>
    /// Discovers all chunks in the chunk set.
    /// </summary>
    public void DiscoverAll()
    {
        foreach (var chunk in this)
            if (chunk is ISkippableChunk s)
                s.Discover();
    }

    /// <summary>
    /// Discovers all chunks in the chunk set in parallel, if <paramref name="parallel"/> is true.
    /// </summary>
    public void DiscoverAll(bool parallel)
    {
        if (!parallel)
        {
            DiscoverAll();
            return;
        }

        Parallel.ForEach(this, chunk =>
        {
            if (chunk is ISkippableChunk s)
                s.Discover();
        });
    }
}
