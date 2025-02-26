﻿namespace GBX.NET.Engines.Plug;

/// <summary>
/// CPlugCamControlModel (0x0910C000)
/// </summary>
[Node(0x0910C000)]
public abstract class CPlugCamControlModel : CMwNod
{
    private CPlugCamShakeModel? shake;

    public CPlugCamShakeModel? Shake { get => shake; set => shake = value; }

    protected CPlugCamControlModel()
    {

    }

    #region 0x000 chunk

    /// <summary>
    /// CPlugCamControlModel 0x000 chunk
    /// </summary>
    [Chunk(0x0910C000)]
    public class Chunk0910C000 : Chunk<CPlugCamControlModel>, IVersionable
    {
        private int version;

        public int Version { get => version; set => version = value; }

        public override void ReadWrite(CPlugCamControlModel n, GameBoxReaderWriter rw)
        {
            rw.Int32(ref version);
            rw.NodeRef<CPlugCamShakeModel>(ref n.shake);
        }
    }

    #endregion
}