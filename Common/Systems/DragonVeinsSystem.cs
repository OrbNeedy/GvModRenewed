using System;
using System.Collections.Generic;
using GvMod.Common.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace GvMod.Common.Systems
{
    public class DragonVeinsSystem : ModSystem
    {
        public List<Point16> veinPoints = new();

        public override void SaveWorldData(TagCompound tag)
        {
            for (int i = 0; i < 7; i++)
            {
                tag[$"VeinPointX{i}"] = veinPoints[i].X;
                tag[$"VeinPointY{i}"] = veinPoints[i].Y;
            }
            base.SaveWorldData(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            for (int i = 0; i < 7; i++)
            {
                if (tag.ContainsKey($"VeinPointX{i}") && tag.ContainsKey($"VeinPointY{i}"))
                {
                    int Xpoint = tag.GetShort($"VeinPointX{i}"), Ypoint = tag.GetShort($"VeinPointX{i}");
                    veinPoints.Add(new Point16(Xpoint, Ypoint));
                } else
                {
                    int attempts = 0;
                    while (attempts < 100)
                    {
                        int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                        int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);

                        if ((Main.tile[x, y].TileType == TileID.Dirt || Main.tile[x, y].TileType == TileID.Stone ||
                            Main.tile[x, y].TileType == TileID.Sandstone || 
                            Main.tile[x, y].TileType == TileID.SnowBlock ||
                            Main.tile[x, y].TileType == TileID.LivingWood ||
                            Main.tile[x, y].TileType == TileID.LivingMahogany || 
                            Main.tile[x, y].TileType == TileID.Mud ||
                            Main.tile[x, y].TileType == TileID.DesertFossil))
                        {
                            if (AddVein(new Point16(x, y))) break;
                        }

                        attempts++;
                    }
                }
            }
            base.LoadWorldData(tag);
        }

        public override void PostUpdatePlayers()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();

                if (adept.septimaType == Players.Sevenths.SeptimaType.None) continue;

                Point16 tilePosition = player.position.ToTileCoordinates16();
                for (int i = 0; i < adept.DragonVeinsVisited.Length; i++)
                {
                    float dX = tilePosition.X - veinPoints[i].X, dY = tilePosition.Y - veinPoints[i].Y;
                    float distance = (float)Math.Sqrt((dX * dX) + (dY * dY));

                    //Main.NewText($"Distance to vein {i}: {distance}");

                    if (distance <= 128)
                    {
                        adept.UpdateInsideDragonVein(i, distance);
                    }

                    if (!adept.DragonVeinsVisited[i] && !NPC.downedAncientCultist)
                    {
                        if (distance <= 32)
                        {
                            adept.DragonVeinsVisited[i] = true;
                            int randomMessage = Main.rand.Next(0, 6);
                            adept.septima.OnVeinVisit(player, adept, i);
                            Main.NewText(Language.GetTextValue($"GvMod.DragonVeins.Visit{randomMessage}"), 
                                adept.septima.MainColor);
                        }
                    }
                }
            }
            base.PostUpdatePlayers();
        }

        public bool AddVein(Point16 point)
        {
            int x = point.X;
            int y = point.Y;

            // ModContent.GetInstance<GvMod>().Logger.Debug($"Adding dragon vein at X: {x}, Y: {y}");

            if (veinPoints.Count >= 7) return false;
            if (veinPoints.Contains(point))
            {
                // ModContent.GetInstance<GvMod>().Logger.Debug($"Vein already stored");
                return false;
            } else
            {
                foreach (Point16 vein in veinPoints)
                {
                    float dX = x - vein.X, dY = y - vein.Y;
                    float distance = (float)Math.Sqrt((dX * dX) + (dY * dY));

                    if (distance <= 128)
                    {
                        //ModContent.GetInstance<GvMod>().Logger.Debug($"Vein too close to a different vein with " +
                        //    $"distance {distance}");
                        return false;
                    }
                }

                if (Main.tile[x, y].TileType == TileID.Dirt || Main.tile[x, y].TileType == TileID.Stone ||
                    Main.tile[x, y].TileType == TileID.Sandstone || Main.tile[x, y].TileType == TileID.SnowBlock ||
                    Main.tile[x, y].TileType == TileID.LivingWood || Main.tile[x, y].TileType == TileID.Mud ||
                    Main.tile[x, y].TileType == TileID.LivingMahogany ||
                    Main.tile[x, y].TileType == TileID.DesertFossil || Main.tile[x, y].TileType == TileID.Crimstone ||
                    Main.tile[x, y].TileType == TileID.Pearlstone || Main.tile[x, y].TileType == TileID.Ebonstone)
                {
                    veinPoints.Add(point);
                    //ModContent.GetInstance<GvMod>().Logger.Debug($"Vein added");
                    return true;
                }
                //ModContent.GetInstance<GvMod>().Logger.Debug($"Tile cannot have a vein");
                return false;
            }
        }
    }
}
