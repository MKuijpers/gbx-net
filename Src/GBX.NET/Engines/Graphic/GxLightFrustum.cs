﻿namespace GBX.NET.Engines.Graphic;

[Node(0x0400A000)]
public class GxLightFrustum : GxLightBall
{
    protected GxLightFrustum()
    {

    }

    [Chunk(0x0400A004)]
    public class Chunk0400A004 : Chunk<GxLightFrustum>
    {
        private int U01;
        private int U02;
        private int U03;
        private float U04;
        private float U05;
        private float U06;
        private float U07;
        private int U08;

        public override void ReadWrite(GxLightFrustum n, GameBoxReaderWriter rw)
        {
            rw.Int32(ref U01);
            rw.Int32(ref U02);
            rw.Int32(ref U03);
            rw.Single(ref U04);
            rw.Single(ref U05);
            rw.Single(ref U06);
            rw.Single(ref U07);
            rw.Int32(ref U08);
        }
    }
}
