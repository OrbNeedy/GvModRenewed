using GvMod.Content.Items.Materials;
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
                    break;
                case NPCID.GoblinTinkerer:
                    shop.Add<HighPerformanceNcGbx>(Condition.Hardmode);
                    break;
            }
            base.ModifyShop(shop);
        }
    }
}
