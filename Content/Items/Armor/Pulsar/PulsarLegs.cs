using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.Pulsar
{
    [AutoloadEquip(EquipType.Legs)]
    public class PulsarLegs : ModItem
    {
        private float bonusSpeed = 16;
        private float bonusCrit = 14;
        private float bonusSecondaryDamage = 15;
        private float bonusOverheatRecovery = 30f;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Red;
            Item.defense = 15;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSpeed, bonusCrit, bonusSecondaryDamage, 
            bonusOverheatRecovery);

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
        }

        public override void UpdateEquip(Player player)
        {
            player.maxRunSpeed *= 1.05f;
            player.moveSpeed += bonusSpeed / 100f;
            player.GetCritChance<SeptimaDamage>() += bonusCrit;
            player.GetDamage<SecondaryAttackDamage>() += bonusSecondaryDamage / 100f;

            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.OverheatRecoveryModifier += bonusOverheatRecovery / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PulsarFragment>(15)
                .AddIngredient(ItemID.LunarBar, 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
