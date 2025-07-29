using GvMod.Common.Players;
using GvMod.Content.Items.Armor.QuillOfficer;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.SumeragiSupervisor
{
    [AutoloadEquip(EquipType.Legs)]
    public class SumeragiSupervisorLegs : ModItem
    {
        private float bonusSpeed = 14;
        private float bonusCrit = 12;
        private float bonusSPRecovery = 18;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 15;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSpeed, bonusCrit, 
            bonusSPRecovery);

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
        }

        public override void UpdateEquip(Player player)
        {
            player.maxRunSpeed *= 1.05f;
            player.moveSpeed += bonusSpeed / 100f;
            player.GetCritChance<SeptimaDamage>() += bonusCrit;

            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.SPRecoveryModifier += bonusSPRecovery / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<QuillOfficerLegs>()
                .AddIngredient<ScarletGoldFragment>(15)
                .AddIngredient(ItemID.Nanites, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
