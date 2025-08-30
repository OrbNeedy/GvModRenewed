using GvMod.Common.Players;
using GvMod.Common.Systems;
using GvMod.Content.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    [AutoloadEquip(EquipType.Face)]
    public class DynamoEye : ModItem
    {
        private int bonusEP = 20;
        private int upgradedBonusEP = 30;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Green;
        }

        public override LocalizedText Tooltip => base.Tooltip;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");
            if (index != -1)
            {
                if (Main.hardMode)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.DynamoEye.UpgradeTooltip",
                        bonusEP + upgradedBonusEP));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.DynamoEye.TrueTooltip", bonusEP));
                }
            }

            if (Main.hardMode)
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
            if (Main.hardMode)
            {
                Item.rare = ItemRarityID.LightRed;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.hardMode)
            {
                Item.rare = ItemRarityID.LightRed;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SeptimaPlayer>().ModifiedMaxEP += bonusEP;

            if (Main.hardMode)
            {
                Item.rare = ItemRarityID.LightRed;

                player.GetModPlayer<SeptimaPlayer>().ModifiedMaxEP += upgradedBonusEP;
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not SealingSpecs && incomingItem.ModItem is not SealingSpecs;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient<BlancCells>(4)
                .AddIngredient<KrippAlloy>(6)
                .AddRecipeGroup(RecipeGroups.CrimtaneBar.ToString(), 2)
                .AddRecipeGroup(RecipeGroups.IronBar.ToString(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
