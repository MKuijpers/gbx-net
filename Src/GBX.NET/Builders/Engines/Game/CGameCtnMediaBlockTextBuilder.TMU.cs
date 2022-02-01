﻿namespace GBX.NET.Builders.Engines.Game;

public partial class CGameCtnMediaBlockTextBuilder
{
    public class TMU : GameBuilder<CGameCtnMediaBlockTextBuilder, CGameCtnMediaBlockText>
    {
        public TMU(CGameCtnMediaBlockTextBuilder baseBuilder, CGameCtnMediaBlockText node) : base(baseBuilder, node) { }

        public override CGameCtnMediaBlockText Build()
        {
            Node.Effect = BaseBuilder.Effect ?? CControlEffectSimi.Create().ForTMUF().Build();
            return Node;
        }
    }
}
