using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.QuillOfficer
{
    [AutoloadEquip(EquipType.Legs)]
    public class QuillOfficerLegs : ModItem
    {
        private float bonusSpeed = 10;
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 9;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSpeed);

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            // speed *= 1.75f;
            // player.maxRunSpeed *= 1.05f;
            //player.maxRunSpeed *= 1.75f;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxRunSpeed *= 1.05f;
            float increase = bonusSpeed / 100f;
            player.moveSpeed += increase;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumLeggings)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<SpiritualStone>(7)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
