using GvMod.Content.Items.Accessories;
using GvMod.Content.Items.Materials;
using System;
using System.Linq;
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
                case ItemID.MoonLordBossBag:
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DjinnBunny>(), 4));
                    break;
            }
        }
    }
}
