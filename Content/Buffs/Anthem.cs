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
            /*Main.NewText("Anthem's Update.");
            Main.NewText($"buffIndex: {buffIndex}");
            for (int i = 0; i < player.buffType.Length; i++)
            {
                Main.NewText($"Buff {i} type: {player.buffType[i]}");
                Main.NewText($"Buff {i} time: {player.buffTime[i]}");
            }*/

            ResurrectionPlayer anthemPlayer = player.GetModPlayer<ResurrectionPlayer>();

            /*Main.NewText($"Anthem type: {ModContent.BuffType<Anthem>()}");
            Main.NewText($"'Anthem' time: {player.buffTime[buffIndex]}");
            Main.NewText($"Resurrection power: {anthemPlayer.resurrectionPower}");
            Main.NewText($"Resurrection state: {anthemPlayer.canResurrect}");*/

            if (player.buffTime[buffIndex] <= 1)
            {
                player.AddBuff(ModContent.BuffType<ResurrectionCooldown>(), 18000);
            }

            /*if (anthemPlayer.resurrectionPower <= 0 || !anthemPlayer.canResurrect)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
                player.AddBuff(ModContent.BuffType<ResurrectionCooldown>(), 18000);
            }*/
        }

        public override bool RightClick(int buffIndex)
        {
            return true;
        }
    }
}
