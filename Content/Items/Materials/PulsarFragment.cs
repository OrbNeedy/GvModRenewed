using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    public class PulsarFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Cyan;

            Item.maxStack = Item.CommonMaxStack;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.AliceBlue.ToVector3() * 0.55f * Main.essScale);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.FragmentSolar)
                .AddIngredient(ItemID.FragmentVortex)
                .AddIngredient(ItemID.FragmentNebula)
                .AddIngredient(ItemID.FragmentStardust)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar, 2)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.FragmentStardust, 2)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar)
                .AddIngredient(ItemID.FragmentVortex)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

            CreateRecipe(2)
                .AddIngredient(ItemID.FragmentNebula, 2)
                .AddIngredient(ItemID.FragmentVortex, 2)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

            CreateRecipe(2)
                .AddIngredient(ItemID.FragmentStardust, 3)
                .AddIngredient(ItemID.FragmentVortex)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
