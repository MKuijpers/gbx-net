﻿namespace GBX.NET.Engines.Game;

/// <summary>
/// Zone genealogy.
/// </summary>
/// <remarks>ID: 0x0311D000</remarks>
[Node(0x0311D000)]
[NodeExtension("ZoneGenealogy")]
public class CGameCtnZoneGenealogy : CMwNod
{
    #region Fields

    private string[]? zoneIds;
    private string? currentZoneId;
    private Direction dir;
    private int currentIndex;

    #endregion

    #region Properties

    [NodeMember]
    [AppliedWithChunk<Chunk0311D001>]
    [AppliedWithChunk<Chunk0311D002>]
    public Direction Dir { get => dir; set => dir = value; }

    [NodeMember]
    [AppliedWithChunk<Chunk0311D001>]
    [AppliedWithChunk<Chunk0311D002>]
    public int CurrentIndex { get => currentIndex; set => currentIndex = value; }

    [NodeMember]
    [AppliedWithChunk<Chunk0311D002>]
    public string[]? ZoneIds { get => zoneIds; set => zoneIds = value; }

    [NodeMember]
    [AppliedWithChunk<Chunk0311D002>]
    public string? CurrentZoneId { get => currentZoneId; set => currentZoneId = value; }

    #endregion

    #region Methods

    public override string ToString()
    {
        return $"{base.ToString()} {{ {(ZoneIds is null ? string.Empty : string.Join(" ", ZoneIds))} }}";
    }

    #endregion

    #region Constructors

    internal CGameCtnZoneGenealogy()
    {

    }

    #endregion

    #region Chunks

    #region 0x001 chunk

    /// <summary>
    /// CGameCtnZoneGenealogy 0x001 chunk
    /// </summary>
    [Chunk(0x0311D001)]
    public class Chunk0311D001 : Chunk<CGameCtnZoneGenealogy>
    {
        public override void ReadWrite(CGameCtnZoneGenealogy n, GameBoxReaderWriter rw)
        {
            rw.Int32(ref n.currentIndex);
            rw.EnumInt32<Direction>(ref n.dir);
        }
    }

    #endregion

    #region 0x002 chunk

    /// <summary>
    /// CGameCtnZoneGenealogy 0x002 chunk
    /// </summary>
    [Chunk(0x0311D002)]
    public class Chunk0311D002 : Chunk<CGameCtnZoneGenealogy>
    {
        public override void ReadWrite(CGameCtnZoneGenealogy n, GameBoxReaderWriter rw)
        {
            rw.ArrayId(ref n.zoneIds);
            rw.Int32(ref n.currentIndex); // 9
            rw.EnumInt32<Direction>(ref n.dir);
            rw.Id(ref n.currentZoneId);
        }
    }

    #endregion

    #endregion
}
