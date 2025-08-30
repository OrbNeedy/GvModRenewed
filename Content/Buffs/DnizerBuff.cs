using GvMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Buffs
{
    public class DnizerBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.DnizerMode = true;
            adept.EPUseModifier *= 0;
            player.GetDamage<SeptimaDamage>() += 0.1f;
            base.Update(player, ref buffIndex);
        }

        public override bool RightClick(int buffIndex)
        {
            return true;
        }
    }
}
