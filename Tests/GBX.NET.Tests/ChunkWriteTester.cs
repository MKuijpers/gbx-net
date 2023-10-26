﻿using System;
using System.IO;
using System.Linq;

namespace GBX.NET.Tests;

internal class ChunkWriteTester<TNode, TChunk> : ChunkTester<TNode, TChunk>, IDisposable
    where TNode : Node where TChunk : Chunk<TNode>
{
    private readonly MemoryStream ms;
    private readonly GameBoxWriter writer;
    private readonly byte[] expectedData;

    public ChunkWriteTester(string gameVersion) : base(gameVersion)
    {
        ms = new MemoryStream();
        writer = new GameBoxWriter(ms, default, default, default, State);
        expectedData = GetChunkData();
    }

    public void Write()
    {
        Chunk.Write(Node, writer);
    }

    public void ReadWriteWithWriter()
    {
        Chunk.ReadWrite(Node, new GameBoxReaderWriter(writer));
    }

    public bool ExpectedDataEqualActualData()
    {
        return expectedData.SequenceEqual(ms.ToArray());
    }

    public override void Dispose()
    {
        base.Dispose();
        writer.Dispose();
        ms.Dispose();
    }
}
