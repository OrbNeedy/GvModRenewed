using GvMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Buffs
{
    public class Anthem : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            ResurrectionPlayer anthemPlayer = player.GetModPlayer<ResurrectionPlayer>();
            anthemPlayer.resurrected = true;

            if (player.buffTime[buffIndex] <= 1)
            {
                player.AddBuff(ModContent.BuffType<ResurrectionCooldown>(), 18000);
            }
        }

        public override bool RightClick(int buffIndex)
        {
            return true;
        }
    }
}
