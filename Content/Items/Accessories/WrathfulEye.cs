using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class WrathfulEye : ModItem
    {
        private float epUseModifier = 10f;
        private float mainDamageModifier = 5f;
        private float upgradedEpUseModifier = 15f;
        private float upgradedMainDamageModifier = 10f;

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
                if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.WrathfulEye.UpgradeTooltip", 
                        mainDamageModifier + upgradedMainDamageModifier, 
                        epUseModifier + upgradedEpUseModifier));
                } else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.WrathfulEye.TrueTooltip", mainDamageModifier,
                        epUseModifier));
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
                Item.rare = ItemRarityID.LightPurple;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                Item.rare = ItemRarityID.LightPurple;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SeptimaPlayer>().EPUseModifier += epUseModifier / 100f;
            player.GetDamage<MainAttackDamage>() += mainDamageModifier / 100f;

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                Item.rare = ItemRarityID.LightPurple;

                player.GetModPlayer<SeptimaPlayer>().EPUseModifier += upgradedEpUseModifier / 100f;
                player.GetDamage<MainAttackDamage>() += upgradedMainDamageModifier / 100f;
            }
        }

        public override void AddRecipes()
        {
            // TODO: Add a Garnet at some point to replace the ruby
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient(ItemID.Ruby, 6)
                .AddIngredient<Nanochip98>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
