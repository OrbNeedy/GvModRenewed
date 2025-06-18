using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.DataStructures;
using GvMod.Content.Tiles;
using Terraria.ID;

namespace GvMod.Common.Systems
{
    public class WorldGenSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int targetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Piles"));

            if (targetIndex != -1)
            {
                tasks.Insert(targetIndex + 1, new UpgradeGenerationPass("GvMod Upgrades", 250f));
            }

            int targetIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Broken Traps")); if (targetIndex != -1)
            {
                tasks.Insert(targetIndex + 1, new DragonVeinsGeneration("GvMod Dragon Veins", 100f));
            }
        }
    }

    public class UpgradeGenerationPass : GenPass
    {
        public UpgradeGenerationPass(string name, double loadWeight) : base(name, loadWeight)
        {
            name = "Generating upgrade capsules";
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.GvMod.Generation.Upgrade1");

            int successes = 0;
            int surfaceSuccesses = 0;
            int attempts = 0;
            int surfaceAttempts = 0;
            int maxSuccesses = (Main.maxTilesX / 250) + (Main.maxTilesY / 250);
            int surfaceMaxSuccesses = (Main.maxTilesX / 900) +(Main.maxTilesY / 900);

            ModContent.GetInstance<GvMod>().Logger.Debug($"Trying to add upgrade items to the world");

            while (successes < maxSuccesses)
            {
                attempts++;
                if (attempts > 1200)
                {
                    break;
                }

                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);

                Tile tile = Framing.GetTileSafely(x, y);

                WorldGen.PlaceTile(x, y, ModContent.TileType<Upgrade1Tile>(), true);

                if (Main.tile[x, y].TileType == ModContent.TileType<Upgrade1Tile>())
                {
                    successes++;
                }
            }

            while (surfaceSuccesses < surfaceMaxSuccesses)
            {
                surfaceAttempts++;
                if (surfaceAttempts > 800)
                {
                    break;
                }

                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                // They can also appear on floating islands, that should be fun 
                int y = WorldGen.genRand.Next(0, (int)GenVars.worldSurfaceHigh);

                Tile tile = Framing.GetTileSafely(x, y);

                WorldGen.PlaceTile(x, y, ModContent.TileType<Upgrade1Tile>(), true);

                if (Main.tile[x, y].TileType == ModContent.TileType<Upgrade1Tile>())
                {
                    successes++;
                }
            }

            ModContent.GetInstance<GvMod>().Logger.Debug($"Added {successes + surfaceSuccesses} upgrades after" +
                $" {attempts + surfaceAttempts} attempts");
        }
    }

    public class DragonVeinsGeneration : GenPass
    {
        public DragonVeinsGeneration(string name, double loadWeight) : base(name, loadWeight)
        {
            name = "Generating dragon veins";
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.GvMod.Generation.Upgrade9");


            int successes = 0;
            int tries = 0; 
            
            ModContent.GetInstance<GvMod>().Logger.Debug($"Generating dragon veins");

            // It will crash the game if for some reason, no valid tiles can be found in 1500 tries
            // I'm sorry if this happens to you, it's very unlikely so just make a new world with a different seed 
            while (successes < 7 && tries < 1500)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);

                //ModContent.GetInstance<GvMod>().Logger.Debug($"Attempt {tries}");

                if (ModContent.GetInstance<DragonVeinsSystem>().AddVein(new Point16(x, y))) successes++;
                tries++;
            }

            ModContent.GetInstance<GvMod>().Logger.Debug($"Generated {successes} dragon veins");
        }
    }
}
