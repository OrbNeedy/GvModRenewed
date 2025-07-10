using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalNPCs
{
    public class NPCValues : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void OnKill(NPC npc)
        {
            if (npc.lastInteraction == 255) return;

            Player player = Main.player[npc.lastInteraction];
            if (player.active && player.GetModPlayer<PlayerBuffs>().AlchemicalField)
            {
                npc.value *= 2;
            }
        }
    }
}
