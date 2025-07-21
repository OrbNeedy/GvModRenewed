using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace GvMod.Content.Tiles
{
    public class KrippAlloyBar : ModTile
    {
        // Tiles such as this are not too important, copied from the Example Mod
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            VanillaFallbackOnModDeletion = TileID.MetalBars;

            AddMapEntry(new Color(31, 153, 92), Language.GetText("MapObject.MetalBar"));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (!WorldGen.SolidTileAllowBottomSlope(i, j + 1))
            {
                WorldGen.KillTile(i, j);
            }
            return true;
        }
    }
}
