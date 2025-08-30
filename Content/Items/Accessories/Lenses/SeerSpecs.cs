using GvMod.Common.Players;
using GvMod.Common.Systems;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    [AutoloadEquip(EquipType.Face)]
    public class SeerSpecs : ModItem
    {
        public float prevasionCostDecrease = 15f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.LightRed;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(prevasionCostDecrease);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PlayerPrevasion>().PrevasionCostModifier -= prevasionCostDecrease / 100f;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not VanishingSpecs &&
                incomingItem.ModItem is not VanishingSpecs;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient<HighPerformanceNcGbx>(4)
                .AddIngredient<BlancCells>(8)
                .AddRecipeGroup(RecipeGroups.EvilBiomeHardmodeMaterial.ToString(), 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
