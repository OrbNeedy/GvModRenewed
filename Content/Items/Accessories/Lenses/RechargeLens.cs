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
    public class RechargeLens : ModItem
    {
        public float epRecovery = 10f;
        public float upgradedEpRecovery = 10f;

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
                if (NPC.downedBoss3)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.RechargeLens.UpgradeTooltip",
                        epRecovery + upgradedEpRecovery));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.RechargeLens.TrueTooltip", epRecovery));
                }
            }

            if (NPC.downedBoss3)
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
            if (NPC.downedBoss3)
            {
                Item.rare = ItemRarityID.Green;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (NPC.downedBoss3)
            {
                Item.rare = ItemRarityID.Green;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.EPRecoveryModifier += epRecovery / 100f;

            if (NPC.downedBoss3)
            {
                Item.rare = ItemRarityID.Green;

                adept.EPRecoveryModifier += upgradedEpRecovery / 100f;
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
                .AddRecipeGroup(RecipeGroups.CopperBar.ToString(), 2)
                .AddIngredient<KrippAlloy>(4)
                .AddRecipeGroup(RecipeGroups.IronBar.ToString(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
