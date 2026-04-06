using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    public class LifeLoupe : ModItem
    {
        private float EPUseDecrease = 10;
        private float EPCooldownIncrease = 25;
        private float DamageDecrease = 15;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Yellow;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(EPUseDecrease, 
            DamageDecrease, EPCooldownIncrease);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PlayerBuffs>().LifeLoupe = true;
            player.GetModPlayer<SeptimaPlayer>().EPUseModifier += EPUseDecrease / 100f;
            player.GetModPlayer<SeptimaPlayer>().EPCooldownModifier += EPCooldownIncrease / 100f;
            player.GetDamage<SeptimaDamage>() -= DamageDecrease / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BlackLens, 2)
                .AddIngredient<PureBlancCells>(16)
                .AddIngredient(ItemID.Nanites, 8)
                .AddIngredient<ScarletGoldFragment>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
