using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    public class Grimoire : ModItem
    {
        public SeptimaType lastSeptima = SeptimaType.None;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Yellow;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (lastSeptima == SeptimaType.None) return;

            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                float pulseAmount = Main.mouseTextColor / 255f;

                Color textColor = SeptimaTemplates.GetSeptimaTemplate(lastSeptima).MainColor * pulseAmount;
                tooltips.Insert(index + 1, new TooltipLine(Mod, "GlaiveSeptimaName",
                    Language.GetText($"Mods.GvMod.ArmedPhenomenon.{lastSeptima.ToString()}.Name").
                    Format(textColor.Hex3())));
                tooltips.Insert(index + 2, new TooltipLine(Mod, "GlaiveSeptima",
                    Language.GetTextValue($"Mods.GvMod.ArmedPhenomenon.{lastSeptima.ToString()}.Description")));
            }
        }

        public override void UpdateEquip(Player player)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            if (adept.septimaType != SeptimaType.None)
            {
                player.GetModPlayer<PlayerBuffs>().ArmedPhenomenonStats = 2;
                player.GetModPlayer<PlayerBuffs>().ArmedPhenomenonVisuals = !hideVisual;

                lastSeptima = adept.septima.Type;
            }
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<PlayerBuffs>().ArmedPhenomenonVisuals = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (player.GetModPlayer<SeptimaPlayer>().septimaType == SeptimaType.None)
            {
                return false;
            }
            return base.CanEquipAccessory(player, slot, modded);
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not Glaive && equippedItem.ModItem is not BindingBrand;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book)
                .AddIngredient(ItemID.ChlorophyteBar, 14)
                .AddIngredient<MirrorShard>()
                .AddIngredient<ScarletGoldFragment>(15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
