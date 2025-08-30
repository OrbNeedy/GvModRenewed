using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Pendants
{
    [AutoloadEquip(EquipType.Neck)]
    public class ChargeguardPendant : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.defense += 2;

            Item.rare = ItemRarityID.Lime;
        }

        public override LocalizedText Tooltip => base.Tooltip;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");
            if (index != -1)
            {
                if (NPC.downedMoonlord)
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.BarrierLocket.UpgradeTooltip"));
                }
                else
                {
                    tooltips[index] = new TooltipLine(Mod, "DynamicTooltip",
                        Language.GetTextValue("Mods.GvMod.Items.BarrierLocket.TrueTooltip"));
                }
            }

            if (NPC.downedMoonlord)
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
            if (NPC.downedMoonlord)
            {
                Item.rare = ItemRarityID.Purple;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (NPC.downedMoonlord)
            {
                Item.rare = ItemRarityID.Purple;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (NPC.downedMoonlord)
            {
                player.GetModPlayer<SeptimaPlayer>().ChargeguardLevel = 2;
            } else
            {
                player.GetModPlayer<SeptimaPlayer>().ChargeguardLevel = 1;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Chain, 6)
                .AddIngredient<Electromagnet>()
                .AddIngredient<HighPerformanceNcGbx>(12)
                .AddIngredient(ItemID.SoulofSight, 6)
                .AddTile(TileID.Mythril)
                .Register();
        }
    }
}
