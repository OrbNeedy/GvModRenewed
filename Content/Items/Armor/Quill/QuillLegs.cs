using GvMod.Content.Items.Armor.Protective;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.Quill
{
    [AutoloadEquip(EquipType.Legs)]
    public class QuillLegs : ModItem
    {
        private float bonusSpeed = 6;
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Green;
            Item.defense = 7;
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
                .AddIngredient(ItemID.HellstoneBar, 2)
                .AddIngredient<ProtectiveLegs>()
                .AddIngredient<KrippAlloy>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
