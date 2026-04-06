using GvMod.Common;
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
    public class BindingBrand : ModItem
    {
        public SeptimaType lastSeptima = SeptimaType.None;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Purple;
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
                player.GetModPlayer<PlayerBuffs>().ArmedPhenomenonStats = 3;
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
            return equippedItem.ModItem is not Grimoire && equippedItem.ModItem is not Glaive;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame,
            Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> glowmask = ModContent.Request<Texture2D>(Texture + "_Glowmask");

            if (lastSeptima == SeptimaType.None) return;

            spriteBatch.Draw(
                glowmask.Value,
                position,
                frame,
                Color.White,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
            base.PostDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor,
            float rotation, float scale, int whoAmI)
        {
            Asset<Texture2D> glowmask = ModContent.Request<Texture2D>(Texture + "_Glowmask");

            if (lastSeptima == SeptimaType.None) return;

            spriteBatch.Draw(
                glowmask.Value,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y - glowmask.Height() * 0.5f
                ),
                new Rectangle(0, 0, glowmask.Width(), glowmask.Height()),
                Color.White,
                rotation,
                glowmask.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 7)
                .AddIngredient<Glaive>()
                .AddIngredient<SpiritualStone>(75)
                .AddIngredient<PureBlancCells>(20)
                .AddCondition(CustomConditions.NearDragonVein)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
