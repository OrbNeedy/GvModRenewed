using GvMod.Common.Players;
using GvMod.Common.Systems;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class OverflashPendant : ModItem
    {
        private float epUseModifier = 0.15f;
        private float mainDamageModifier = 0.15f;

        public override void SetDefaults()
        {
            Item.accessory = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(mainDamageModifier, epUseModifier);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SeptimaPlayer>().EPUseModifier += epUseModifier;
            player.GetDamage<MainAttackDamage>() += mainDamageModifier;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Chain, 2)
                .AddIngredient<KrippAlloy>(8)
                .AddRecipeGroup(RecipeGroups.CrimtaneBar.ToString(), 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
