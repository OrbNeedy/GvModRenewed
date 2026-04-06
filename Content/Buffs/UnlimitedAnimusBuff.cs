using GvMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Buffs
{
    class UnlimitedAnimusBuff : ModBuff
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
            PlayerBuffs buffs = player.GetModPlayer<PlayerBuffs>();
            buffs.UnlimitedAnimus = true;
        }

        public override bool RightClick(int buffIndex)
        {
            return true;
        }
    }
}
