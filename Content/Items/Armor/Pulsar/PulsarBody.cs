using GvMod.Content.Items.Materials;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using GvMod.Common.Players;

namespace GvMod.Content.Items.Armor.Pulsar
{
    [AutoloadEquip(EquipType.Body)]
    public class PulsarBody : ModItem
    {
        private float bonusMainDamage = 15f;
        private float bonusEPRecovery = 16f;
        private float bonusEPUse = 20f;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Red;
            Item.defense = 20;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusMainDamage, bonusEPRecovery, bonusEPUse);

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MainAttackDamage>() += bonusMainDamage / 100f;

            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.EPRecoveryModifier += bonusEPRecovery / 100f;
            adept.EPUseModifier -= bonusEPUse / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PulsarFragment>(20)
                .AddIngredient(ItemID.LunarBar, 16)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
