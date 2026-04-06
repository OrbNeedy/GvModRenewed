using GvMod.Content.Items.Corpses;
using GvMod.Content.Items.Materials;
using GvMod.Content.Items.Tools;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalNPCs
{
    public class NPCStores : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            switch (shop.NpcType)
            {
                case NPCID.Mechanic:
                    shop.Add<Electromagnet>(Condition.DownedMechBossAll);
                    shop.Add<HighPerformanceNcGbx>(Condition.DownedGoblinArmy, Condition.Hardmode);
                    break;
                case NPCID.Merchant:
                    shop.Add<Nanochip98>(Condition.DownedEyeOfCthulhu);
                    shop.Add<CorpsePreservingGuide>();
                    break;
                case NPCID.Pirate:
                    shop.Add<PigronCorpse>(Condition.DownedDukeFishron);
                    break;
                case NPCID.GoblinTinkerer:
                    shop.Add<HighPerformanceNcGbx>(Condition.Hardmode);
                    break;
                case NPCID.Dryad:
                    shop.Add<FlyingEyeCorpse>(Condition.BloodMoon);
                    shop.Add<WanderingEyeCorpse>(Condition.EclipseOrBloodMoon, Condition.Hardmode);
                    break;
                case NPCID.SkeletonMerchant:
                    shop.Add<CorpsePreservingGuide>();
                    break;
                case NPCID.WitchDoctor:
                    shop.Add<ScorpionCorpse>();
                    shop.Add<FlyingSnakeCorpse>(Condition.DownedGolem);
                    break;
            }
            base.ModifyShop(shop);
        }
    }
}
