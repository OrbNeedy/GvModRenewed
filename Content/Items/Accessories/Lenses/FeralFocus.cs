using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    public class FeralFocus : ModItem
    {
        public float bonusDamage = 10f;
        public float upgradedBonusDamage = 5f;

        public float criticalHealthBonusDamage = 20f;
        
        public float extraEpUse = 15f;
        public float upgradedExtraEpUse = 7f;

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
                if (NPC.downedGolemBoss)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.FeralFocus.UpgradeTooltip",
                        bonusDamage + upgradedBonusDamage, criticalHealthBonusDamage, 
                        extraEpUse + upgradedExtraEpUse));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.FeralFocus.TrueTooltip", bonusDamage, 
                        extraEpUse));
                }
            }

            if (NPC.downedGolemBoss)
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
            if (NPC.downedGolemBoss)
            {
                Item.rare = ItemRarityID.Yellow;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (NPC.downedGolemBoss)
            {
                Item.rare = ItemRarityID.Yellow;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            
            if (player.statLife <= player.statLifeMax2 / 2)
            {
                player.GetDamage<SeptimaDamage>() += bonusDamage / 100f;

                adept.EPUseModifier += extraEpUse / 100f;

                if (NPC.downedGolemBoss)
                {
                    adept.EPUseModifier += upgradedExtraEpUse / 100f;

                    player.GetDamage<SeptimaDamage>() += upgradedBonusDamage / 100f;

                    if (player.statLife <= player.statLifeMax2 / 5)
                    {
                        player.GetDamage<SeptimaDamage>() += criticalHealthBonusDamage / 100f;
                    }
                }
            }

            if (NPC.downedGolemBoss)
            {
                Item.rare = ItemRarityID.Yellow;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient<BlancCells>(2)
                .AddIngredient(ItemID.Bone, 12)
                .AddIngredient<KrippAlloy>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
