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
            int[] woodChests = { ModContent.ItemType<Stage1Upgrade>() };
            int[] goldChestItems = { ModContent.ItemType<Stage1Upgrade>(), 
                ModContent.ItemType<Stage2Upgrade>() };
            int[] lockedGoldChestItems = { ModContent.ItemType<Stage2Upgrade>() };
            int[] shadowChestItems = { ModContent.ItemType<Stage2Upgrade>(), 
                ModContent.ItemType<Stage3Upgrade>() };

            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                int chestItemsChoice = 0;
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers)
                {
                    switch (Main.tile[chest.x, chest.y].TileFrameX)
                    {
                        // Wood chest
                        case 0:
                            PutInChest(chest, ref chestItemsChoice, woodChests, 
                                !WorldGen.genRand.NextBool(16), 1, 8);
                            break;
                        // Gold chest
                        case 1 * 36:
                            PutInChest(chest, ref chestItemsChoice, goldChestItems, 
                                !WorldGen.genRand.NextBool(60), 1, 6);
                            break;
                        // Locked gold chest
                        case 2 * 36:
                            PutInChest(chest, ref chestItemsChoice, lockedGoldChestItems, 
                                !WorldGen.genRand.NextBool(14), 2, 10);
                            break;
                        // Shadow chest
                        case 3 * 36:
                            PutInChest(chest, ref chestItemsChoice, shadowChestItems,
                                !WorldGen.genRand.NextBool(14), 1, 5);
                            break;
                        // Locked Shadow chest
                        case 4 * 36:
                            break;
                        // Vine chest
                        case 12 * 36:
                            PutInChest(chest, ref chestItemsChoice, woodChests,
                                !WorldGen.genRand.NextBool(10), 2, 10);
                            break;
                        // Spider chest
                        case 16 * 36:
                            break;
                        // Ocean chest
                        case 18 * 36:
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
