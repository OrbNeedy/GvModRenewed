using GvMod.Common.Players;
using GvMod.Common.Systems;
using GvMod.Content.Items.Upgrades;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class DynamoEye : ModItem
    {
        private int bonusEP = 20;

        public override void SetDefaults()
        {
            Item.accessory = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusEP);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SeptimaPlayer>().ModifiedMaxEP += bonusEP;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient<Stage1Upgrade>()
                .AddRecipeGroup(RecipeGroups.EvilMaterial.ToString(), 4)
                .AddRecipeGroup(RecipeGroups.IronBar.ToString(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
