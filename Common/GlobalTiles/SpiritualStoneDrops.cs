using GvMod.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalTiles
{
    public class SpiritualStoneDrops : GlobalTile
    {
        public override void Drop(int i, int j, int type)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.HasTile)
            {
                int chance = -1;
                switch (tile.TileType)
                {
                    case TileID.Stone:
                        chance = 275;
                        break;
                    case TileID.Dirt:
                        chance = 400;
                        break;
                    case TileID.Pearlstone:
                        chance = 125;
                        break;
                    case TileID.Crimstone:
                    case TileID.Ebonstone:
                        chance = 450;
                        break;
                }

                if (chance > 0 && WorldGen.genRand.NextBool(chance))
                {
                    Vector2 pos = new Point16(i, j).ToWorldCoordinates();
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle((int)pos.X,
                        (int)pos.Y, 16, 16), ModContent.ItemType<SpiritualStone>(),
                        WorldGen.genRand.Next(1, 6));
                }
            }
            base.Drop(i, j, type);
        }
    }
}
