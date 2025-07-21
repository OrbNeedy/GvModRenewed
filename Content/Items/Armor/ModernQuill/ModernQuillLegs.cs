using GvMod.Common.Players;
using GvMod.Content.Items.Armor.Protective;
using GvMod.Content.Items.Armor.Quill;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.ModernQuill
{
    [AutoloadEquip(EquipType.Legs)]
    public class ModernQuillLegs : ModItem
    {
        private float bonusSpeed = 12;
        private float bonusCrit = 8;
        private float bonusEPRecovery = 10;
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.defense = 12;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSpeed, bonusCrit, 
            bonusEPRecovery);

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            // speed *= 1.75f;
            // player.maxRunSpeed *= 1.05f;
            // player.maxRunSpeed *= 1.75f;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxRunSpeed *= 1.05f;
            player.moveSpeed += bonusSpeed / 100f;
            player.GetCritChance<SeptimaDamage>() += bonusCrit;
            player.GetModPlayer<SeptimaPlayer>().EPRecoveryModifier += bonusEPRecovery / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 4)
                .AddIngredient<QuillLegs>()
                .AddIngredient<Electromagnet>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
