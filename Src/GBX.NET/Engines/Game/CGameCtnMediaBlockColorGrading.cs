﻿namespace GBX.NET.Engines.Game;

/// <summary>
/// MediaTracker block - Color grading (0x03186000)
/// </summary>
[Node(0x03186000)]
[NodeExtension("GameCtnMediaBlockColorGrading")]
public partial class CGameCtnMediaBlockColorGrading : CGameCtnMediaBlock, CGameCtnMediaBlock.IHasKeys
{
    #region Fields

    private FileRef image;
    private IList<Key> keys;

    #endregion

    #region Properties

    IEnumerable<CGameCtnMediaBlock.Key> IHasKeys.Keys
    {
        get => keys.Cast<CGameCtnMediaBlock.Key>();
        set => keys = value.Cast<Key>().ToList();
    }

    [NodeMember]
    public FileRef Image
    {
        get => image;
        set => image = value;
    }

    [NodeMember]
    public IList<Key> Keys
    {
        get => keys;
        set => keys = value;
    }

    #endregion

    #region Constructors

    protected CGameCtnMediaBlockColorGrading()
    {
        image = null!;
        keys = null!;
    }

    #endregion

    #region Chunks

    #region 0x000 chunk

    /// <summary>
    /// CGameCtnMediaBlockColorGrading 0x000 chunk
    /// </summary>
    [Chunk(0x03186000)]
    public class Chunk03186000 : Chunk<CGameCtnMediaBlockColorGrading>
    {
        public override void ReadWrite(CGameCtnMediaBlockColorGrading n, GameBoxReaderWriter rw)
        {
            rw.FileRef(ref n.image!);
        }
    }

    #endregion

    #region 0x001 chunk

    /// <summary>
    /// CGameCtnMediaBlockColorGrading 0x001 chunk
    /// </summary>
    [Chunk(0x03186001)]
    public class Chunk03186001 : Chunk<CGameCtnMediaBlockColorGrading>
    {
        public override void ReadWrite(CGameCtnMediaBlockColorGrading n, GameBoxReaderWriter rw)
        {
            rw.ListKey(ref n.keys!);
        }
    }

    #endregion

    #endregion
}
