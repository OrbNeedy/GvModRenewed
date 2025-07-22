using GvMod.Content.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.Systems
{
    public class RecipeModifications : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                if (recipe.createItem.type == ItemID.AvengerEmblem)
                {
                    recipe.AddIngredient<SeptimaEmblem>()
                        .AddIngredient(ItemID.SoulofMight, 5)
                        .AddIngredient(ItemID.SoulofSight, 5)
                        .AddIngredient(ItemID.SoulofFright, 5)
                        .Register();
                }
            }
        }
    }
}
