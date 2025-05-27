using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Content.Items.Upgrades;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalNPCs
{
    public class ItemDrops : GlobalNPC
    {
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            base.ModifyGlobalLoot(globalLoot);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.boss)
            {
                npcLoot.Add(ItemDropRule.ExpertGetsRerolls(ModContent.ItemType<CapacityUpgrade>(), 500, 3));
            }

            switch (npc.type)
            {
                case NPCID.EaterofWorldsHead:
                case NPCID.BrainofCthulhu:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NagaUpgrade>(), 2));
                    break;
                case NPCID.SkeletronHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TechnosUpgrade>(), 2));
                    break;
                case NPCID.SkeletronPrime:
                case NPCID.TheDestroyer:
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrochiUpgrade>(), 8));
                    break;
                case NPCID.Plantera:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MizuchiUpgrade>(), 2));
                    break;
                case NPCID.CultistBoss:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VasukiUpgrade>(), 2));
                    break;
                case NPCID.DukeFishron:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OuroborosUpgrade>()));
                    break;
            }
            base.ModifyNPCLoot(npc, npcLoot);
        }
    }
}
