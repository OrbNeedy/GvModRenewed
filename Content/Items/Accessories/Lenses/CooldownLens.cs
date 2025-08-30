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
    public class CooldownLens : ModItem
    {
        public float cooldownRecovery = 12f;
        public float upgradedCooldownRecovery = 13f;

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
                if (NPC.downedBoss2)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.CooldownLens.UpgradeTooltip",
                        cooldownRecovery + upgradedCooldownRecovery));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.CooldownLens.TrueTooltip", cooldownRecovery));
                }
            }

            if (NPC.downedBoss2)
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
            if (NPC.downedBoss2)
            {
                Item.rare = ItemRarityID.Green;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (NPC.downedBoss2)
            {
                Item.rare = ItemRarityID.Green;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.EPRecoveryModifier += cooldownRecovery / 100f;

            if (NPC.downedBoss2)
            {
                Item.rare = ItemRarityID.Green;

                adept.EPRecoveryModifier += upgradedCooldownRecovery / 100f;
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
                .AddIngredient<KrippAlloy>(6)
                .AddRecipeGroup(RecipeGroups.SilverBar.ToString(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
