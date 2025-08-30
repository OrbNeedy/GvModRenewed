using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    public class LatencyLens : ModItem
    {
        public float bonusSecondaryDamage = 22f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.LightRed;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSecondaryDamage);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            if (adept.Overheated)
            {
                player.GetDamage<SecondaryAttackDamage>() += bonusSecondaryDamage / 100f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient<Nanochip98>(8)
                .AddIngredient<BlancCells>(10)
                .AddIngredient(ItemID.HellstoneBar, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
