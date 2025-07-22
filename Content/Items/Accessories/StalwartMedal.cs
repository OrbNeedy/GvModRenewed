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
    public class StalwartMedal : ModItem
    {
        private int damageReduction = 5;
        private int bonusDefense = 4;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Green;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageReduction, bonusDefense);

        public override void UpdateEquip(Player player)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.GetModPlayer<SeptimaPlayer>().Overheated)
            {
                player.endurance += damageReduction / 100f;
                player.statDefense += bonusDefense;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 2)
                .AddIngredient<SpiritualStone>(6)
                .AddRecipeGroup(RecipeGroups.GoldBar.ToString(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
