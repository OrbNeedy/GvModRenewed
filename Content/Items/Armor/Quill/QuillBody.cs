using GvMod.Common.Players;
using GvMod.Content.Items.Armor.Protective;
using GvMod.Content.Items.Upgrades;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.Quill
{
    [AutoloadEquip(EquipType.Body)]
    public class QuillBody : ModItem
    {
        private float bonusSeptimaDamage = 12;
        private float bonusOverheatRecovery = 15;
        private float bonusEPRecovery = 10;
        public LocalizedText SetBonusText { get; private set; }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Green;
            Item.defense = 11;

            SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(bonusEPRecovery,
                bonusOverheatRecovery);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSeptimaDamage);

        public override void UpdateEquip(Player player)
        {
            float increase = bonusSeptimaDamage / 100f;
            player.GetDamage<SeptimaDamage>() += increase;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<QuillLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            float increase = bonusOverheatRecovery / 100f;
            float increase2 = bonusEPRecovery / 100f;
            player.setBonus = SetBonusText.Value;
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.OverheatRecoveryModifier += increase;
            adept.EPRecoveryModifier += increase2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 7)
                .AddIngredient<ProtectiveBody>()
                .AddIngredient<Stage1Upgrade>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
