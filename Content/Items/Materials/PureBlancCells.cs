using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    internal class PureBlancCells : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Cyan;

            Item.maxStack = Item.CommonMaxStack;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.45f * Main.essScale);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient<BlancCells>(9)
                .AddIngredient(ItemID.Ectoplasm)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

            CreateRecipe(2)
                .AddIngredient<BlancCells>(6)
                .AddIngredient(ItemID.ChlorophyteBar, 2)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
