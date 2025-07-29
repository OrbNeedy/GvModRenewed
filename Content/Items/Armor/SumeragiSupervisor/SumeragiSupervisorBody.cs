using GvMod.Common.Players;
using GvMod.Content.Items.Armor.QuillOfficer;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.SumeragiSupervisor
{
    [AutoloadEquip(EquipType.Body)]
    public class SumeragiSupervisorBody : ModItem
    {
        // Base
        private float bonusSeptimaDamage = 24;
        private float bonusEPUse = 14;
        private float bonusEPRecovery = 18;
        // Set
        private float bonusSpecialDamage = 22;
        private int prevasionDamageLimit = 18;
        private float prevasionLifeLimit = 5;
        private int prevasionCost = 18;

        public LocalizedText SetBonusText { get; private set; }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 20;

            SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(prevasionLifeLimit,
                prevasionDamageLimit, prevasionCost, bonusSpecialDamage);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSeptimaDamage, bonusEPUse, 
            bonusEPRecovery);

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SeptimaDamage>() += bonusSeptimaDamage / 100f;

            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.EPUseModifier -= bonusEPUse / 100f;
            adept.EPRecoveryModifier += bonusEPRecovery / 100f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<SumeragiSupervisorLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SetBonusText.Value;
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            player.GetDamage<SpecialAttackDamage>() += bonusSpecialDamage / 100;

            PlayerPrevasion prevasion = player.GetModPlayer<PlayerPrevasion>();

            prevasion.PrevasionLifeLimit = prevasionLifeLimit / 100;
            prevasion.PrevasionDamageLimit = prevasionDamageLimit;
            prevasion.PrevasionCost = prevasionCost;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<QuillOfficerBody>()
                .AddIngredient<ScarletGoldFragment>(5)
                .AddIngredient(ItemID.Nanites, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
