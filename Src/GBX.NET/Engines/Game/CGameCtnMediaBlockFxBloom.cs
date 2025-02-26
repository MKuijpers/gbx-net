﻿namespace GBX.NET.Engines.Game;

/// <summary>
/// MediaTracker block - Bloom effect (0x03083000)
/// </summary>
/// <remarks>Bloom MediaTracker block for TMUF and older games. This node causes "Couldn't load map" in ManiaPlanet.</remarks>
[Node(0x03083000)]
[NodeExtension("GameCtnMediaBlockFxBloom")]
public partial class CGameCtnMediaBlockFxBloom : CGameCtnMediaBlockFx, CGameCtnMediaBlock.IHasKeys
{
    #region Fields

    private IList<Key> keys;

    #endregion

    #region Constructors

    protected CGameCtnMediaBlockFxBloom()
    {
        keys = null!;
    }

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

    #region Chunks

    #region 0x001 chunk

    /// <summary>
    /// CGameCtnMediaBlockFxBloom 0x001 chunk
    /// </summary>
    [Chunk(0x03083001)]
    public class Chunk03083001 : Chunk<CGameCtnMediaBlockFxBloom>
    {
        public override void ReadWrite(CGameCtnMediaBlockFxBloom n, GameBoxReaderWriter rw)
        {
            rw.ListKey(ref n.keys!);
        }
    }

    #endregion
    
    #endregion
}
