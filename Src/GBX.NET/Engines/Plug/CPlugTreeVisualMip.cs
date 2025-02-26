﻿namespace GBX.NET.Engines.Plug;

[Node(0x09015000)]
public class CPlugTreeVisualMip : CPlugTree
{
    private IDictionary<float, CPlugTree> levels;

    public IDictionary<float, CPlugTree> Levels
    {
        get => levels;
        set => levels = value;
    }

    protected CPlugTreeVisualMip()
    {
        levels = null!;
    }

    [Chunk(0x09015002)]
    public class Chunk09015002 : Chunk<CPlugTreeVisualMip>
    {
        public override void ReadWrite(CPlugTreeVisualMip n, GameBoxReaderWriter rw)
        {
            rw.DictionaryNode(ref n.levels!, overrideKey: true);
        }

        public override async Task ReadWriteAsync(CPlugTreeVisualMip n, GameBoxReaderWriter rw, ILogger? logger, CancellationToken cancellationToken = default)
        {
            n.levels = (await rw.DictionaryNodeAsync(n.levels!, overrideKey: true, cancellationToken))!;
        }
    }
}
