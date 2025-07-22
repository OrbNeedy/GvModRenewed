using GvMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class WrathfulEye : ModItem
    {
        private float epUseModifier = 0.2f;
        private float mainDamageModifier = 0.3f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.LightPurple;
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
                .AddIngredient<OverflashPendant>()
                .AddIngredient(ItemID.BlackLens, 2)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<OverflashPendant>()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient(ItemID.SoulofNight, 12)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
