﻿namespace GBX.NET.Engines.Hms;

/// <remarks>ID: 0x06003000</remarks>
[Node(0x06003000)]
public class CHmsItem : CMwNod
{
    private CPlugSolid solid;

    [NodeMember(ExactlyNamed = true)]
    [AppliedWithChunk<Chunk06003001>]
    public CPlugSolid Solid { get => solid; set => solid = value; }

    internal CHmsItem()
    {
        solid = null!;
    }

    #region Chunks

    #region 0x001 chunk (solid)

    /// <summary>
    /// CHmsItem 0x001 chunk (solid)
    /// </summary>
    [Chunk(0x06003001, "solid")]
    public class Chunk06003001 : Chunk<CHmsItem>
    {
        public override void ReadWrite(CHmsItem n, GameBoxReaderWriter rw)
        {
            rw.NodeRef<CPlugSolid>(ref n.solid!);
        }
    }

    #endregion

    #region 0x010 chunk

    /// <summary>
    /// CHmsItem 0x010 chunk
    /// </summary>
    [Chunk(0x06003010)]
    public class Chunk06003010 : Chunk<CHmsItem>
    {
        private ulong U01;
        private short U02;

        public override void ReadWrite(CHmsItem n, GameBoxReaderWriter rw)
        {
            rw.UInt64(ref U01);
            rw.Int16(ref U02);
        }
    }

    #endregion

    #region 0x011 chunk

    /// <summary>
    /// CHmsItem 0x011 chunk
    /// </summary>
    [Chunk(0x06003011)]
    public class Chunk06003011 : Chunk<CHmsItem>
    {
        private ulong U01;
        private short U02;

        public override void ReadWrite(CHmsItem n, GameBoxReaderWriter rw)
        {
            rw.UInt64(ref U01);
            rw.Int16(ref U02);
        }
    }

    #endregion

    #endregion
}
