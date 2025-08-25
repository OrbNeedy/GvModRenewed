using GvMod.Common.Systems;
using GvMod.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    public class KrippAlloy : ModItem
    {
        public override void SetStaticDefaults()
        {
            
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;
            Item.DefaultToPlaceableTile(ModContent.TileType<KrippAlloyBar>());

            Item.maxStack = Item.CommonMaxStack;
        }

        public override void AddRecipes()
        {
            CreateRecipe(8)
                .AddRecipeGroup(RecipeGroups.CopperBar.ToString())
                .AddRecipeGroup(RecipeGroups.SilverBar.ToString())
                .AddIngredient<Kripp>(5)
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
}
