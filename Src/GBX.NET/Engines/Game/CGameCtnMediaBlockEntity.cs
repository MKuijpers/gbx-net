﻿namespace GBX.NET.Engines.Game;

/// <summary>
/// MediaTracker block - Entity (0x0329F000)
/// </summary>
[Node(0x0329F000)]
public class CGameCtnMediaBlockEntity : CGameCtnMediaBlock
{
    private CPlugEntRecordData recordData;

    public CPlugEntRecordData RecordData
    {
        get => recordData;
        set => recordData = value;
    }

    protected CGameCtnMediaBlockEntity()
    {
        recordData = null!;
    }

    [Chunk(0x0329F000)]
    public class Chunk0329F000 : Chunk<CGameCtnMediaBlockEntity>, IVersionable
    {
        private int version;

        public Vec3 U01;
        public int[]? U02;
        public bool? U03;
        public bool? U04;
        public bool? U05;
        public bool? U06;
        public Vec3? U07;
        public Ident? U08;
        public Vec3? U09;
        public int? U10;
        public bool? U11;
        public int? U12;
        public Vec3 U13;
        public int U14;
        public string? U15;
        public (string, string)[]? U16;
        public string[]? U17;

        public int Version
        {
            get => version;
            set => version = value;
        }

        public override void ReadWrite(CGameCtnMediaBlockEntity n, GameBoxReaderWriter rw)
        {
            rw.Int32(ref version);
            rw.NodeRef<CPlugEntRecordData>(ref n.recordData!);

            if (version > 3)
            {
                rw.UntilFacade(Unknown);
                return;
            }

            rw.Vec3(ref U01);
            rw.Array<int>(ref U02);

            if (version >= 2)
            {
                rw.Boolean(ref U03);
                rw.Boolean(ref U04);
                rw.Boolean(ref U05);
                rw.Boolean(ref U06);
                rw.Vec3(ref U07);

                if (version >= 3)
                {
                    rw.Ident(ref U08);
                    rw.Vec3(ref U09);
                    rw.Int32(ref U10); // array of PackDesc?
                    rw.Boolean(ref U11);

                    if (U11 == true)
                    {
                        rw.Int32(ref U12);

                        // NGameBadge::BadgeArchive
                        rw.Vec3(ref U13);

                        if (U12 == 0)
                        {
                            rw.Int32(ref U14);
                            rw.String(ref U15);
                        }

                        rw.Array(ref U16, (i, r) => (r.ReadString(), r.ReadString()),
                        (x, w) =>
                        {
                            w.Write(x.Item1);
                            w.Write(x.Item2);
                        });

                        rw.ArrayString(ref U17);
                    }
                }
            }
        }
    }
}
