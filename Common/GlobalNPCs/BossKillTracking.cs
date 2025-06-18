using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalNPCs
{
    public class BossKillTracking : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (!npc.boss) return;

            // Main.NewText("Boss killed");
            if (Main.netMode == NetmodeID.Server)
            {
                // Main.NewText("Net mode: Server");
                foreach (Player player in Main.ActivePlayers)
                {
                    // Main.NewText($"Checking player {player.name}");
                    if (npc.playerInteraction[player.whoAmI])
                    {
                        // Main.NewText("Queueing stage check");
                        SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
                        adept.QueueStageCheck = true;
                        adept.septima.OnBossDefeat(npc.type, player, adept);
                    }
                }
            } else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                // Main.NewText("Net mode: Single Player");
                // Main.NewText("Queueing stage check");
                SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
                adept.QueueStageCheck = true;
                adept.septima.OnBossDefeat(npc.type, Main.LocalPlayer, adept);
            }
            base.OnKill(npc);
        }
    }
}
