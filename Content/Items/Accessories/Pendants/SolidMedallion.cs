using GvMod.Common.Players;
using GvMod.Common.Systems;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Pendants
{
    [AutoloadEquip(EquipType.Neck)]
    public class SolidMedallion : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.defense = 4;

            Item.rare = ItemRarityID.Green;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override void UpdateEquip(Player player)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.GetModPlayer<SeptimaPlayer>().Overheated)
            {
                player.noKnockback = true;
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not SolidStalwartPendant && 
                incomingItem.ModItem is not SolidStalwartPendant;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Chain, 4)
                .AddIngredient<Nanochip98>(8)
                .AddRecipeGroup(RecipeGroups.EvilMaterial.ToString(), 6)
                .AddRecipeGroup(RecipeGroups.GoldBar.ToString(), 4)
                .AddRecipeGroup(RecipeGroups.IronBar.ToString(), 4)
                .AddRecipeGroup(RecipeGroups.CrimtaneBar.ToString(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
