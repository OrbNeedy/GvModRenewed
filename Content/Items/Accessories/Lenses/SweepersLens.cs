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
    public class SweepersLens : ModItem
    {
        private float bonusDamage = 7f;
        private float upgradedBonusDamage = 11f;
        private float anthemBonusDamage = 12f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Orange;
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
                        Language.GetTextValue("Mods.GvMod.Items.SweepersLens.UpgradeTooltip", bonusDamage +
                        upgradedBonusDamage, anthemBonusDamage));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.SweepersLens.TrueTooltip", bonusDamage));
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
            player.GetDamage<SecondaryAttackDamage>() += bonusDamage / 100f;

            if (NPC.downedPlantBoss)
            {
                Item.rare = ItemRarityID.Lime;

                player.GetDamage<SecondaryAttackDamage>() += upgradedBonusDamage / 100f;

                if (player.GetModPlayer<ResurrectionPlayer>().resurrected)
                {
                    player.GetDamage<SecondaryAttackDamage>() += anthemBonusDamage / 100f;
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
                .AddIngredient<BlancCells>(6)
                .AddRecipeGroup(RecipeGroups.EvilMaterial.ToString(), 12)
                .AddIngredient<KrippAlloy>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
