using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    [AutoloadEquip(EquipType.Face)]
    public class AdrenalineLens : ModItem
    {
        public float SPSaveChance = 20f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.LightPurple;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SPSaveChance);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SeptimaPlayer>().SPSaveChance = SPSaveChance / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BlackLens, 2)
                .AddIngredient<HighPerformanceNcGbx>(6)
                .AddIngredient<KrippAlloy>(6)
                .AddIngredient(ItemID.HallowedBar, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
