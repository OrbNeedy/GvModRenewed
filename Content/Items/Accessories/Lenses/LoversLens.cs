using GvMod.Common.Players;
using GvMod.Common.Systems;
using GvMod.Content.Items.Accessories.Pendants;
using GvMod.Content.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    [AutoloadEquip(EquipType.Face)]
    public class LoversLens : ModItem
    {
        private float bonusDamage = 7f;
        private float upgradedBonusDamage = 8f;
        private float anthemBonusDamage = 10f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Blue;
        }

        public override LocalizedText Tooltip => base.Tooltip;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");
            if (index != -1)
            {
                if (NPC.downedPlantBoss)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.LoversLens.UpgradeTooltip", bonusDamage + 
                        upgradedBonusDamage, anthemBonusDamage));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.LoversLens.TrueTooltip", bonusDamage));
                }
            }

            if (NPC.downedPlantBoss)
            {
                index = tooltips.FindLastIndex((x) => x.Name.StartsWith("ItemName") && x.Mod == "Terraria");
                if (index != -1)
                {
                    tooltips[index].Text = base.DisplayName.Value + " +";
                }
            }
        }

        public override void PostUpdate()
        {
            if (NPC.downedPlantBoss)
            {
                Item.rare = ItemRarityID.Lime;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (NPC.downedPlantBoss)
            {
                Item.rare = ItemRarityID.Lime;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<MainAttackDamage>() += bonusDamage / 100f;

            if (NPC.downedPlantBoss)
            {
                Item.rare = ItemRarityID.Lime;

                player.GetDamage<MainAttackDamage>() += upgradedBonusDamage / 100f;

                if (player.GetModPlayer<ResurrectionPlayer>().resurrected)
                {
                    player.GetDamage<MainAttackDamage>() += anthemBonusDamage / 100f;
                }
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not SoulmateSight &&
                incomingItem.ModItem is not SoulmateSight;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient<SpiritualStone>(8)
                .AddIngredient<Nanochip98>(4)
                .AddRecipeGroup(RecipeGroups.IronBar.ToString(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
