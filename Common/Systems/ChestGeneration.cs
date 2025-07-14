using GvMod.Content.Items.Upgrades;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.Systems
{
    public class ChestGeneration : ModSystem
    {
        public override void PostWorldGen()
        {
            int s1Index = ModContent.ItemType<Stage1Upgrade>();
            int s2Index = ModContent.ItemType<Stage2Upgrade>();
            int s3Index = ModContent.ItemType<Stage3Upgrade>();
            int s4Index = ModContent.ItemType<Stage4Upgrade>();
            int s5Index = ModContent.ItemType<Stage5Upgrade>();
            int universalIndex = ModContent.ItemType<UniversalUpgradeItem>();
            int capacityIndex = ModContent.ItemType<CapacityUpgrade>();

            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                int chestItemsChoice = 0;
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers)
                {

                    PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                        !WorldGen.genRand.NextBool(1250));

                    switch (Main.tile[chest.x, chest.y].TileFrameX)
                    {
                        // Wood chest
                        case 0:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index }, 
                                !WorldGen.genRand.NextBool(2), 1, 5);
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index },
                                !WorldGen.genRand.NextBool(10), 1, 5);
                            break;
                        // Gold chest
                        case 1 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index } ,
                                !WorldGen.genRand.NextBool(14), 4, 9);
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index },
                                !WorldGen.genRand.NextBool(6), 4, 11);
                            break;
                        // Locked gold chest
                        case 2 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index },
                                !WorldGen.genRand.NextBool(4), 1, 13);
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(6), 4, 9);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(20));
                            break;
                        // Shadow chest
                        case 3 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index, s3Index },
                                !WorldGen.genRand.NextBool(10), 1, 6);
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(4), 4, 13);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(38));
                            break;
                        // Locked Shadow chest
                        case 4 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index, s3Index },
                                !WorldGen.genRand.NextBool(10), 1, 6);
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(4), 4, 13);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(38));
                            break;
                        // Barrel
                        case 5 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(40), 1, 21);
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s5Index },
                                !WorldGen.genRand.NextBool(60), 1, 51);
                            break;
                        // Trashcan
                        case 6 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index, s3Index, 
                                s4Index, s5Index }, !WorldGen.genRand.NextBool(10), 10, 11);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                !WorldGen.genRand.NextBool(50), 1, 21);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(50));
                            break;
                        // Ebonwood chest
                        case 7 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index, s3Index },
                                !WorldGen.genRand.NextBool(14), 1, 13);
                            break;
                        // Rich Mahogany chest
                        case 8 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index },
                                !WorldGen.genRand.NextBool(6), 4, 11);
                            break;
                        // Pearlwood chest
                        case 9 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index },
                                !WorldGen.genRand.NextBool(8), 1, 11);
                            break;
                        // Ivy (Jungle Shrine) chest
                        case 10 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index },
                                !WorldGen.genRand.NextBool(6), 1, 9);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(45));
                            break;
                        // Frozen chest
                        case 11 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index },
                                !WorldGen.genRand.NextBool(10), 1, 9);
                            break;
                        // Living Wood chest
                        case 12 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index },
                                !WorldGen.genRand.NextBool(8), 1, 6);
                            break;
                        // Skyware chest
                        case 13 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index },
                                !WorldGen.genRand.NextBool(6), 1, 6);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(35));
                            break;
                        // Shadewood chest
                        case 14 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index, s3Index },
                                !WorldGen.genRand.NextBool(12), 1, 13);
                            break;
                        // Spider chest
                        case 15 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s3Index },
                                !WorldGen.genRand.NextBool(12), 1, 11);
                            break;
                        // Lihzahrd chest
                        case 16 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index },
                                !WorldGen.genRand.NextBool(8), 1, 11);
                            PutInChest(chest, ref chestItemsChoice, new[] { s4Index },
                                !WorldGen.genRand.NextBool(2), 1, 3);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(38));
                            break;
                        // Ocean chest
                        case 17 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index },
                                !WorldGen.genRand.NextBool(6), 1, 6);
                            PutInChest(chest, ref chestItemsChoice, new[] { s5Index },
                                !WorldGen.genRand.NextBool(120));
                            break;
                        // Dungeon Jungle chest
                        case 18 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Dungeon Corruption chest
                        case 19 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Dungeon Crimson chest
                        case 20 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Dungeon Hallow chest
                        case 21 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Dungeon Frozen chest
                        case 22 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Locked Dungeon Jungle chest
                        case 23 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Locked Dungeon Corruption chest
                        case 24 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Locked Dungeon Crimson chest
                        case 25 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Locked Dungeon Hallow chest
                        case 26 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Locked Dungeon Frozen chest
                        case 27 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Locked Dungeon Frozen chest
                        case 29 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index, s4Index },
                                !WorldGen.genRand.NextBool(), 1, 16);
                            PutInChest(chest, ref chestItemsChoice, new[] { universalIndex },
                                false, 5, 6);
                            break;
                        // Dynasty chest
                        case 30 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index },
                                !WorldGen.genRand.NextBool(50), 1, 6);
                            break;
                        // Honey chest
                        case 31 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index },
                                !WorldGen.genRand.NextBool(2), 1, 11);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(65));
                            break;
                        // Steampunk chest
                        case 32 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(4), 1, 9);
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index },
                                !WorldGen.genRand.NextBool(8), 1, 5);
                            break;
                        // Palm Wood chest
                        case 33 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(5), 1, 11);
                            break;
                        // Shroom chest
                        case 34 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index },
                                !WorldGen.genRand.NextBool(2), 1, 11);
                            break;
                        // Boreal Wood chest
                        case 35 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index },
                                !WorldGen.genRand.NextBool(6), 1, 6);
                            break;
                        // Slime chest
                        case 36 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index },
                                !WorldGen.genRand.NextBool(6), 1, 6);
                            break;
                        // Green Dungeon chest
                        case 37 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(12), 1, 9);
                            break;
                        // Locked Green Dungeon chest
                        case 38 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(12), 1, 9);
                            break;
                        // Pink Dungeon chest
                        case 39 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(12), 1, 9);
                            break;
                        // Locked Pink Dungeon chest
                        case 40 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(12), 1, 9);
                            break;
                        // Blue Dungeon chest
                        case 41 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(12), 1, 9);
                            break;
                        // Locked Blue Dungeon chest
                        case 42 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(12), 1, 9);
                            break;
                        // Bone chest
                        case 43 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(12), 1, 9);
                            break;
                        // Cactus chest
                        case 44 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index },
                                !WorldGen.genRand.NextBool(8), 1, 6);
                            break;
                        // Flesh chest
                        case 45 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index, s3Index },
                                !WorldGen.genRand.NextBool(8), 1, 9);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(12));
                            break;
                        // Obsidian chest
                        case 46 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(6), 1, 9);
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index },
                                !WorldGen.genRand.NextBool(10), 1, 6);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(36));
                            break;
                        // Pumpkin chest
                        case 47 * 36:
                            break;
                        // Spooky chest
                        case 48 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s4Index },
                                !WorldGen.genRand.NextBool(4), 1, 11);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(28));
                            break;
                        // Glass chest
                        case 49 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index, s3Index, 
                                s4Index, s5Index, universalIndex, capacityIndex }, 
                                !WorldGen.genRand.NextBool(100));
                            break;
                        // Martian chest
                        case 50 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index, s5Index, 
                                universalIndex },!WorldGen.genRand.NextBool(10), 1, 6);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(10));
                            break;
                        // Meteorite chest
                        case 51 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index, s3Index },
                                !WorldGen.genRand.NextBool(8), 1, 5);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(40));
                            break;
                        // Granite chest
                        case 52 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index },
                                !WorldGen.genRand.NextBool(2), 1, 5);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(8));
                            break;
                        // Marble chest
                        case 53 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index },
                                !WorldGen.genRand.NextBool(2), 1, 5);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(8));
                            break;
                        // Crystal chest
                        case 54 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s2Index },
                                !WorldGen.genRand.NextBool(6));
                            PutInChest(chest, ref chestItemsChoice, new[] { s3Index },
                                !WorldGen.genRand.NextBool(2));
                            PutInChest(chest, ref chestItemsChoice, new[] { s4Index },
                                !WorldGen.genRand.NextBool(24));
                            break;
                        // Golden chest
                        case 55 * 36:
                            PutInChest(chest, ref chestItemsChoice, new[] { s1Index, s2Index, s3Index,
                                s4Index, s5Index, universalIndex }, !WorldGen.genRand.NextBool(60), 1, 26);
                            PutInChest(chest, ref chestItemsChoice, new[] { capacityIndex },
                                !WorldGen.genRand.NextBool(100));
                            break;
                    }
                }
            }
        }

        private void PutInChest(Chest chest, ref int chestItemsChoice, int[] itemPool, bool skip)
        {
            if (skip) return;
            for (int inventoryIndex = 0; inventoryIndex < chest.item.Length; inventoryIndex++)
            {
                if (chest.item[inventoryIndex].type == ItemID.None)
                {
                    chestItemsChoice = (chestItemsChoice + 1) % itemPool.Length;
                    chest.item[inventoryIndex].SetDefaults(itemPool[chestItemsChoice]);
                    break;
                }
            }
        }

        private void PutInChest(Chest chest, ref int chestItemsChoice, int[] itemPool, bool skip, 
            int minStack = 1, int maxStack = 2)
        {
            if (skip) return;
            for (int inventoryIndex = 0; inventoryIndex < chest.item.Length; inventoryIndex++)
            {
                if (chest.item[inventoryIndex].type == ItemID.None)
                {
                    chestItemsChoice = (chestItemsChoice + 1) % itemPool.Length;
                    chest.item[inventoryIndex].SetDefaults(itemPool[chestItemsChoice]);
                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(minStack, maxStack);
                    break;
                }
            }
        }
    }
}
