﻿namespace GBX.NET.Engines.Game;

/// <summary>
/// MediaTracker block - Camera blend effect (0x0316D000)
/// </summary>
[Node(0x0316D000)]
[NodeExtension("GameCtnMediaBlockFxCameraBlend")]
public partial class CGameCtnMediaBlockFxCameraBlend : CGameCtnMediaBlock, CGameCtnMediaBlock.IHasKeys
{
    #region Fields

    private IList<Key> keys;

    #endregion

    #region Properties

    IEnumerable<CGameCtnMediaBlock.Key> IHasKeys.Keys
    {
        get => keys.Cast<CGameCtnMediaBlock.Key>();
        set => keys = value.Cast<Key>().ToList();
    }

    [NodeMember]
    public IList<Key> Keys
    {
        get => keys;
        set => keys = value;
    }

    #endregion

    #region Constructors

    protected CGameCtnMediaBlockFxCameraBlend()
    {
        keys = null!;
    }

    #endregion

    #region Chunks

    #region 0x000 chunk

    [Chunk(0x0316D000)]
    public class Chunk0316D000 : Chunk<CGameCtnMediaBlockFxCameraBlend>, IVersionable
    {
        private int version;

        public int Version
        {
            get => version;
            set => version = value;
        }

        public override void ReadWrite(CGameCtnMediaBlockFxCameraBlend n, GameBoxReaderWriter rw)
        {
            rw.Int32(ref version);
            rw.ListKey(ref n.keys!);
        }
    }

    #endregion

    #endregion
}
