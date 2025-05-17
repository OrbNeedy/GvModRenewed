using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Ammo
{
    public class HairDart : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 10;

            Item.damage = 1;
            Item.DamageType = DamageClass.Default;

            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 0, 0, 10);
            Item.rare = ItemRarityID.Green;

            Item.ammo = Item.type;
        }

        public override void AddRecipes()
        {
            CreateRecipe(99)
                .AddRecipeGroup(RecipeGroupID.IronBar)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
