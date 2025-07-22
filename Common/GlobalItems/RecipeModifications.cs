using GvMod.Content.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalItems
{
    public class RecipeModifications : GlobalItem
    {
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.AvengerEmblem)
                .AddIngredient<SeptimaEmblem>()
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
