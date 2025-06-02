using System.Collections.Generic;
using GvMod.Common.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalNPCs
{
    public class SeptimaEffectLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.BeetleBuff);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player.DeadOrGhost) return;

            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            if (adept.septima == null) return;

            adept.septima.DrawPassive(ref drawInfo, player, adept);
            if (adept.UsingMainSkill)
            {
                adept.septima.DrawAttack(ref drawInfo, player, adept);
            }
        }
    }
}
