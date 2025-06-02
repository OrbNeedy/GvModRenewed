using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Buffs
{
    public class ResurrectionCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //Main.NewText("ResurrectionCooldown's Update.");
            if (player.HasBuff<Anthem>())
            {
                player.ClearBuff(ModContent.BuffType<Anthem>());
            }
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}
