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
    public class StalwartMedal : ModItem
    {
        private float damageReduction = 7;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.defense = 4;

            Item.rare = ItemRarityID.Green;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageReduction);

        public override void UpdateEquip(Player player)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.GetModPlayer<SeptimaPlayer>().Overheated)
            {
                player.endurance += damageReduction / 100f;
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
                .AddIngredient(ItemID.Silk, 6)
                .AddRecipeGroup(RecipeGroups.CopperBar.ToString(), 4)
                .AddIngredient<KrippAlloy>(10)
                .AddRecipeGroup(RecipeGroups.GoldBar.ToString(), 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
