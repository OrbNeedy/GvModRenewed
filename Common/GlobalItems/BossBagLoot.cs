using GvMod.Content.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalItems
{
    public class BossBagLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                case ItemID.WallOfFleshBossBag:
                    foreach (var rule in itemLoot.Get())
                    {
                        if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && 
                            oneFromOptionsDrop.dropIds.Contains(ItemID.WarriorEmblem))
                        {
                            var original = oneFromOptionsDrop.dropIds.ToList();
                            original.Add(ModContent.ItemType<SeptimaEmblem>());
                            oneFromOptionsDrop.dropIds = original.ToArray();
                        }
                    }
                    break;
            }
        }
    }
}
