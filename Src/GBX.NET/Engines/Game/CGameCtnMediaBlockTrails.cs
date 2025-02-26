﻿namespace GBX.NET.Engines.Game;

/// <summary>
/// MediaTracker block - Trails (0x030A9000)
/// </summary>
[Node(0x030A9000)]
[NodeExtension("GameCtnMediaBlockTrails")]
public class CGameCtnMediaBlockTrails : CGameCtnMediaBlock, CGameCtnMediaBlock.IHasTwoKeys
{
    #region Fields

    private TimeSingle start = TimeSingle.Zero;
    private TimeSingle end = TimeSingle.FromSeconds(3);

    #endregion

    #region Properties

    [NodeMember]
    public TimeSingle Start
    {
        get => start;
        set => start = value;
    }

    [NodeMember]
    public TimeSingle End
    {
        get => end;
        set => end = value;
    }

    #endregion

    #region Constructors

    protected CGameCtnMediaBlockTrails()
    {

    }

    #endregion

    #region Chunks

    #region 0x000 chunk

    [Chunk(0x030A9000)]
    public class Chunk030A9000 : Chunk<CGameCtnMediaBlockTrails>
    {
        public override void ReadWrite(CGameCtnMediaBlockTrails n, GameBoxReaderWriter rw)
        {
            rw.TimeSingle(ref n.start);
            rw.TimeSingle(ref n.end);
        }
    }

    #endregion

    #endregion
}
