﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GBX.NET.Engines.Game
{
    [Node(0x03081000)]
    public class CGameCtnMediaBlockFxBlurDepth : CGameCtnMediaBlockFx
    {
        public Key[] Keys { get; set; }

        public CGameCtnMediaBlockFxBlurDepth(ILookbackable lookbackable, uint classID) : base(lookbackable, classID)
        {

        }

        [Chunk(0x03081001)]
        public class Chunk03081001 : Chunk<CGameCtnMediaBlockFxBlurDepth>
        {
            public override void Read(CGameCtnMediaBlockFxBlurDepth n, GameBoxReader r, GameBoxWriter unknownW)
            {
                n.Keys = r.ReadArray(i =>
                {
                    return new Key()
                    {
                        Time = r.ReadSingle(),
                        LensSize = r.ReadSingle(),
                        ForceFocus = r.ReadBoolean(),
                        FocusZ = r.ReadSingle(),
                    };
                });
            }

            public override void Write(CGameCtnMediaBlockFxBlurDepth n, GameBoxWriter w, GameBoxReader unknownR)
            {
                w.Write(n.Keys, x =>
                {
                    w.Write(x.Time);
                    w.Write(x.LensSize);
                    w.Write(x.ForceFocus);
                    w.Write(x.FocusZ);
                });
            }
        }

        public class Key : MediaBlockKey
        {
            public float LensSize { get; set; }
            public bool ForceFocus { get; set; }
            public float FocusZ { get; set; }
        }
    }
}
