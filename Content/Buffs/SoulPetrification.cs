using GvMod.Common.GlobalNPCs;
using GvMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Buffs
{
    public class SoulPetrification : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<DebuffNPC>().soulPetrified = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerDebuffs>().soulPetrified = true;
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            return true;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return true;
        }
    }
}
