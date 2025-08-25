using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Content.Items.Materials;
using GvMod.Content.Items.Upgrades;
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
    public class Glaive : ModItem
    {
        public SeptimaType lastSeptima = SeptimaType.None;

        public override void SetDefaults()
        {
            Item.accessory = true;


            Item.rare = ItemRarityID.Lime;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (lastSeptima == SeptimaType.None) return; 

            int index = tooltips.FindLastIndex((x) => x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index != -1)
            {
                float pulseAmount = Main.mouseTextColor / 255f;

                Color textColor = SeptimaPlayer.GetStaticSeptima(lastSeptima).MainColor * pulseAmount;
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
                player.GetModPlayer<PlayerBuffs>().ArmedPhenomenonStats = 1;
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

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, 
            Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> glowmask = ModContent.Request<Texture2D>(Texture + "_Glowmask");

            if (lastSeptima == SeptimaType.None) return;

            Color color = SeptimaPlayer.GetSeptima(lastSeptima).MainColor;

            spriteBatch.Draw(
                glowmask.Value,
                position,
                frame,
                color,
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

            Color color = SeptimaPlayer.GetSeptima(lastSeptima).MainColor;

            spriteBatch.Draw(
                glowmask.Value,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y - glowmask.Height() * 0.5f
                ),
                new Rectangle(0, 0, glowmask.Width(), glowmask.Height()),
                color,
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
                .AddIngredient(ItemID.HellstoneBar, 7)
                .AddIngredient(ItemID.IronBroadsword)
                .AddIngredient<SpiritualStone>(15)
                .AddIngredient<BlancCells>(20)
                .AddTile(TileID.Anvils)
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
}
