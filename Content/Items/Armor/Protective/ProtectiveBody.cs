using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.Protective
{
    [AutoloadEquip(EquipType.Body)]
    public class ProtectiveBody : ModItem
    {
        private float increaseInSeptimaDamage = 5;
        public LocalizedText SetBonusText { get; private set; }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 6;

            SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(increaseInSeptimaDamage);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(increaseInSeptimaDamage);

        public override void UpdateEquip(Player player)
        {
            float increase = increaseInSeptimaDamage / 100f;
            player.GetDamage<SeptimaDamage>() += increase;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<ProtectiveLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            float increase = increaseInSeptimaDamage / 100f;
            player.setBonus = SetBonusText.Value;
            player.GetDamage<SeptimaDamage>() += increase;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 7)
                .AddRecipeGroup("GoldBar", 5)
                .AddIngredient(ItemID.Silk, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
