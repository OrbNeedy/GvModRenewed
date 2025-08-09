using GvMod.Common.Players;
using GvMod.Content.Items.Armor.QuillOfficer;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.DragonSaviors
{
    [AutoloadEquip(EquipType.Body)]
    public class DragonSaviorsBody : ModItem
    {
        // Base
        private float bonusSeptimaDamage = 28;
        private float bonusMainDamage = 22;
        private float bonusEPUse = 16;
        private float bonusEPRecovery = 20;
        // Set
        private float bonusSPrecovery = 18;
        private int prevasionDamageLimit = 20;
        private float prevasionLifeLimit = 8;
        private int prevasionCost = 18;

        public LocalizedText SetBonusText { get; private set; }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 24;

            SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(prevasionLifeLimit,
                prevasionDamageLimit, prevasionCost, bonusSPrecovery);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSeptimaDamage, bonusEPUse,
            bonusEPRecovery, bonusMainDamage);

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SeptimaDamage>() += bonusSeptimaDamage / 100f;
            player.GetDamage<MainAttackDamage>() += bonusMainDamage / 100f;

            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.EPUseModifier -= bonusEPUse / 100f;
            adept.EPRecoveryModifier += bonusEPRecovery / 100f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<DragonSaviorsLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SetBonusText.Value;
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.SPRecoveryModifier += bonusSPrecovery / 100f;

            player.GetModPlayer<SetBonusPlayer>().DragonSaviorsBonus = true;

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
