using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class ChargeguardPendant : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.defense += 2;

            Item.rare = ItemRarityID.Lime;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override void UpdateEquip(Player player)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SeptimaPlayer>().ChargeguardLevel = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Chain, 2)
                .AddIngredient<SpiritualStone>(8)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddTile(TileID.Mythril)
                .Register();
        }
    }
}
