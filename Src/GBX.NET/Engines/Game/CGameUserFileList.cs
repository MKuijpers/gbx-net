﻿namespace GBX.NET.Engines.Game;

[Node(0x031B7000)]
public partial class CGameUserFileList : CMwNod
{
    private IList<FileInfo> files;

    public IList<FileInfo> Files { get => files; set => files = value; }

    protected CGameUserFileList()
    {
        files = null!;
    }

    #region 0x000 chunk

    /// <summary>
    /// CGameUserFileList 0x000 chunk
    /// </summary>
    [Chunk(0x031B7000)]
    public class Chunk031B7000 : Chunk<CGameUserFileList>, IVersionable
    {
        private int version;

        public int Version { get => version; set => version = value; }

        public override void ReadWrite(CGameUserFileList n, GameBoxReaderWriter rw)
        {
            rw.Int32(ref version);

            // SGameUserFileInfo array
            rw.List<FileInfo>(ref n.files!, FileInfo.ReadWrite);
        }
    }

    #endregion
}
