using GvMod.Content.Items.Accessories;
using GvMod.Content.Items.Materials;
using GvMod.Content.Items.Pickups;
using GvMod.Content.Items.Upgrades;
using System.Linq;
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
            globalLoot.Add(ItemDropRule.ByCondition(new RarityDropCondition(4), 
                ModContent.ItemType<UniversalUpgradeItem>(), 800));
            globalLoot.Add(ItemDropRule.ByCondition(new MinLifeDropCondition(100),
                ModContent.ItemType<UniversalUpgradeItem>(), 600));
            base.ModifyGlobalLoot(globalLoot);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.boss)
            {
                npcLoot.Add(ItemDropRule.ByCondition(new MirrorShardDropCondition(), 
                    ModContent.ItemType<MirrorShard>(), 4));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<UniversalUpgradeItem>(), 250, 1, 5));
                npcLoot.Add(ItemDropRule.ByCondition(new SeptimaLumpCondition(),
                    ModContent.ItemType<SeptimaLump>(), 1, 10, 20));
            } else
            {
                npcLoot.Add(ItemDropRule.ByCondition(new SeptimaLumpCondition(),
                    ModContent.ItemType<SeptimaLump>(), 10));
            }

            if (npc.boss || npc.rarity >= 4)
            {
                npcLoot.Add(ItemDropRule.ExpertGetsRerolls(ModContent.ItemType<CapacityUpgrade>(), 500, 3));
                npcLoot.Add(ItemDropRule.ExpertGetsRerolls(ModContent.ItemType<UpgradeComponent>(), 300, 3));
            }

            // Skeletons
            if (NPCID.Sets.Skeletons[npc.type])
            {
                npcLoot.Add(ItemDropRule.ByCondition(new AfterPlanteraDropCondition(), 
                    ModContent.ItemType<Stage4Upgrade>(), 38, 2, 12));
            }

            switch (npc.type)
            {
                // Bosses
                case NPCID.EyeofCthulhu:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NagaUpgrade>(), 2));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stage1Upgrade>(), 12, 1, 10));
                    break;
                case NPCID.SkeletronHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TechnosUpgrade>(), 2));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stage2Upgrade>(), 12, 1, 10));
                    break;
                case NPCID.WallofFlesh:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CapacityUpgrade>(), 250, 3, 10));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stage3Upgrade>(), 18, 1, 10));
                    foreach (var rule in npcLoot.Get())
                    {
                        if (rule is DropBasedOnExpertMode dropBasedOnExpertMode &&
                            dropBasedOnExpertMode.ruleForNormalMode is
                            OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop &&
                            oneFromOptionsDrop.dropIds.Contains(ItemID.WarriorEmblem))
                        {
                            var original = oneFromOptionsDrop.dropIds.ToList();
                            original.Add(ModContent.ItemType<SeptimaEmblem>());
                            oneFromOptionsDrop.dropIds = original.ToArray();
                        }
                    }
                    break;
                case NPCID.SkeletronPrime:
                case NPCID.TheDestroyer:
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VasukiUpgrade>(), 8));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stage3Upgrade>(), 12, 1, 7));
                    break;
                case NPCID.Plantera:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrochiUpgrade>(), 2));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stage4Upgrade>(), 40, 5, 15));
                    break;
                case NPCID.Golem:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stage4Upgrade>(), 12, 1, 10));
                    break;
                case NPCID.CultistBoss:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MizuchiUpgrade>(), 2));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stage4Upgrade>(), 20, 1, 5));
                    break;
                case NPCID.DukeFishron:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OuroborosUpgrade>(), 2));
                    break;
                case NPCID.LunarTowerNebula:
                case NPCID.LunarTowerSolar:
                case NPCID.LunarTowerStardust:
                case NPCID.LunarTowerVortex:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PulsarFragment>(), 1, 3, 15));
                    break;
                case NPCID.MoonLordCore:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DullahanUpgrade>(), 2));
                    break;

                // Non-bosses
                case NPCID.Ghost:
                case NPCID.Poltergeist:
                    npcLoot.Add(ItemDropRule.ByCondition(new AfterPlanteraDropCondition(), 
                        ModContent.ItemType<Stage4Upgrade>(), 25, 5, 20));
                    break;
                case NPCID.GoblinWarrior:
                case NPCID.GoblinPeon:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SeptimaStabilizer>(), 60));
                    break;
                case NPCID.Lihzahrd:
                case NPCID.LihzahrdCrawler:
                case NPCID.FlyingSnake:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScarletGoldFragment>(), 28));
                    break;
            }
            base.ModifyNPCLoot(npc, npcLoot);
        }
    }
}
