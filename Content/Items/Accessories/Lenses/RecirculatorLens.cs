using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using GvMod.Content.Items.Upgrades;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    public class RecirculatorLens : ModItem
    {
        public float prevasionNoCostChance = 15f;
        public float upgradedPrevasionNoCostChance = 10f;

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
                if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.RecirculatorLens.UpgradeTooltip",
                        prevasionNoCostChance + upgradedPrevasionNoCostChance));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.RecirculatorLens.TrueTooltip", 
                        prevasionNoCostChance));
                }
            }

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
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
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                Item.rare = ItemRarityID.Pink;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                Item.rare = ItemRarityID.Pink;
            }
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PlayerPrevasion>().PrevasionCostAvoidanceChance = prevasionNoCostChance / 
                100f;

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                Item.rare = ItemRarityID.Pink;

                player.GetModPlayer<PlayerPrevasion>().PrevasionCostAvoidanceChance += 
                    upgradedPrevasionNoCostChance / 100f;
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not VanishingSpecs && 
                incomingItem.ModItem is not VanishingSpecs;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient<KrippAlloy>(6)
                .AddIngredient<Stage1Upgrade>()
                .AddIngredient(ItemID.HellstoneBar, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
