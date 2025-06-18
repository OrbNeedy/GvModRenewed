using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Enums;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using GvMod.Content.Items.Upgrades;

namespace GvMod.Content.Tiles
{
    public class Upgrade1Tile : ModTile
    {
        private Asset<Texture2D> texture;
        private readonly int animationFrameWidth = 18;

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            DustType = -1; // No dust when mined.
            //RegisterItemDrop();

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = false;
            /*  This is what is copied from the Campfire tile
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.StyleWrapLimit = 16;
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.WaterDeath = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.DrawYOffset = 2;
			*/
            TileObjectData.newTile.StyleLineSkip = 9; // This needs to be added to work for modded tiles.
            TileObjectData.addTile(Type);

            // Etc
            // AddMapEntry(new Color(254, 121, 2), Language.GetText("ItemName.Campfire"));

            // Assets
            //texture = ModContent.Request<Texture2D>(Texture + "_Flame");

            RegisterItemDrop(ModContent.ItemType<Stage1Upgrade>(), 0);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.10196f * 0.5f;
            g = 0.03921f * 0.5f;
            b = 0.8f * 0.5f;
        }

        // This method allows you to determine whether or not the tile will draw itself flipped in the world
        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            // Flips the sprite if x coord is odd. Makes the tile more interesting
            if (i % 2 == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            // Tweak the frame drawn by x position so tiles next to each other are off-sync and look much more interesting
            int uniqueAnimationFrame = Main.tileFrame[Type] + i;
            if (i % 2 == 0)
                uniqueAnimationFrame += 2;
            if (i % 3 == 0)
                uniqueAnimationFrame += 2;
            if (i % 4 == 0)
                uniqueAnimationFrame += 2;
            uniqueAnimationFrame %= 3; // Mod operation

            // frameYOffset = modTile.AnimationFrameHeight * Main.tileFrame[type] will already be set before this hook is called
            // But we have a horizontal animated texture, so we use frameXOffset instead of frameYOffset
            frameXOffset = uniqueAnimationFrame * animationFrameWidth;
        }

        public override bool KillSound(int i, int j, bool fail)
        {
            // Play the glass shattering sound instead of the normal digging sound if the tile is destroyed on this hit
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.Tink, new Vector2(i, j).ToWorldCoordinates());
                return false;
            }
            return base.KillSound(i, j, fail);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 1)
            {
                frameCounter = 0;
                frame = ++frame % 3;
            }
        }

        /*public override void GetTileFlameData(int i, int j, ref TileDrawing.TileFlameData tileFlameData)
        {
            tileFlameData.flameTexture = flameTexture.Value;
            tileFlameData.flameColor = new Color(200, 200, 200, 0);
            tileFlameData.flameCount = 1;
        }*/

    }
}
