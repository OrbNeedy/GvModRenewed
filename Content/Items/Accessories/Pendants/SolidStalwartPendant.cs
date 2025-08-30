using GvMod.Common.Players;
using GvMod.Common.Systems;
using GvMod.Content.Items.Accessories.Lenses;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Pendants
{
    public class SolidStalwartPendant : ModItem
    {
        private float damageReduction = 12;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.defense = 8;

            Item.rare = ItemRarityID.Orange;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageReduction);

        public override void UpdateEquip(Player player)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.GetModPlayer<SeptimaPlayer>().Overheated)
            {
                player.noKnockback = true;
                player.endurance += damageReduction / 100f;
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not SolidMedallion && equippedItem.ModItem is not StalwartMedal 
                && incomingItem.ModItem is not SolidMedallion && incomingItem.ModItem is not StalwartMedal;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SolidMedallion>()
                .AddIngredient<StalwartMedal>()
                .AddIngredient(ItemID.Wire, 20)
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddTile(TileID.Anvils)
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
}
