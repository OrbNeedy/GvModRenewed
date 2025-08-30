using GvMod.Common.Players;
using GvMod.Common.Systems;
using GvMod.Content.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Pendants
{
    [AutoloadEquip(EquipType.Neck)]
    public class OverflashPendant : ModItem
    {
        private float epUseModifier = 15f;
        private float upgradedEpUseModifier = -5f;
        private float mainDamageModifier = 15f;
        private float upgradedMainDamageModifier = 7f;

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
                if (Main.hardMode)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.Overflash.UpgradeTooltip",
                        epUseModifier + upgradedEpUseModifier, 
                        mainDamageModifier + upgradedMainDamageModifier));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.Overflash.TrueTooltip", epUseModifier, 
                        mainDamageModifier));
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
                Item.rare = ItemRarityID.Pink;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.hardMode)
            {
                Item.rare = ItemRarityID.Pink;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SeptimaPlayer>().EPUseModifier += epUseModifier / 100f;
            player.GetDamage<MainAttackDamage>() += mainDamageModifier / 100f;

            if (Main.hardMode)
            {
                Item.rare = ItemRarityID.Pink;

                player.GetModPlayer<SeptimaPlayer>().EPUseModifier += upgradedEpUseModifier / 100f;
                player.GetDamage<MainAttackDamage>() += upgradedMainDamageModifier / 100f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Chain, 2)
                .AddIngredient<KrippAlloy>(8)
                .AddRecipeGroup(RecipeGroups.CopperBar.ToString(), 6)
                .AddRecipeGroup(RecipeGroups.CrimtaneBar.ToString(), 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
