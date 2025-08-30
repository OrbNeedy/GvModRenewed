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
    public class Reviberoptic : ModItem
    {
        public float penaltyRecovery = 15f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Green;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(penaltyRecovery);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PlayerBuffs>().Reviberoptics = true;

            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.EPRecoveryModifier -= penaltyRecovery / 100f;
            adept.OverheatRecoveryModifier -= penaltyRecovery / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient<BlancCells>(6)
                .AddIngredient<KrippAlloy>(4)
                .AddRecipeGroup(RecipeGroups.IronBar.ToString(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
