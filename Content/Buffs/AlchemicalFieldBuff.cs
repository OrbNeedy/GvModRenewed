using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GvMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Buffs
{
    public class AlchemicalFieldBuff : ModBuff
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
            player.GetModPlayer<PlayerBuffs>().AlchemicalField = true;
        }

        public override bool RightClick(int buffIndex)
        {
            return true;
        }
    }
}
