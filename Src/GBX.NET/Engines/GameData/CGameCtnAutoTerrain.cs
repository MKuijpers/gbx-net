﻿namespace GBX.NET.Engines.GameData;

/// <summary>
/// CGameCtnAutoTerrain (0x03120000)
/// </summary>
[Node(0x03120000)]
public class CGameCtnAutoTerrain : CMwNod
{
    protected CGameCtnAutoTerrain()
    {

    }

    [Chunk(0x03120001)]
    public class Chunk03120001 : Chunk<CGameCtnAutoTerrain>
    {
        public override void Read(CGameCtnAutoTerrain n, GameBoxReader r)
        {
            var offset = r.ReadInt3();
            var genealogy = r.ReadNodeRef<CGameCtnZoneGenealogy>();
        }
    }
}
