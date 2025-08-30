using GvMod.Common.Players;
using GvMod.Common.Systems;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    public class DilationReticle : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.LightRed;
        }

        public override LocalizedText Tooltip => base.Tooltip;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PlayerBuffs>().DilationReticles = true;
        }

        public override void AddRecipes()
        {
            // TODO: Figure out what the recipe is in GV2
            CreateRecipe()
                .AddIngredient(ItemID.BlackLens, 2)
                .AddIngredient<BlancCells>(4)
                .AddRecipeGroup(RecipeGroups.TitaniumBar.ToString(), 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
