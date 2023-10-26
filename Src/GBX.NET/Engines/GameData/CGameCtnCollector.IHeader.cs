﻿using System.Drawing;

namespace GBX.NET.Engines.GameData;

public partial class CGameCtnCollector
{
    public interface IHeader : INodeHeader<CGameCtnCollector>
    {
        public Ident Ident { get; set; }
        public string PageName { get; set; }
        public ECollectorFlags Flags { get; set; }
        public int CatalogPosition { get; set; }
        public string? Name { get; set; }
        public EProdState? ProdState { get; set; }
        public Color[,]? Icon { get; set; }
        public byte[]? IconWebP { get; set; }
        public ulong FileTime { get; set; }
    }
}
