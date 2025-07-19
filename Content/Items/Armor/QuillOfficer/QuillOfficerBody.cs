using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.QuillOfficer
{
    [AutoloadEquip(EquipType.Body)]
    public class QuillOfficerBody : ModItem
    {
        private float bonusSeptimaDamage = 20;
        private float bonusOverheatRecovery = 20;
        private float bonusEPUse = 12;
        private int prevasionDamageLimit = 17;
        private float prevasionLifeLimit = 4;
        private int prevasionCost = 25;
        public LocalizedText SetBonusText { get; private set; }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 13;

            SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(prevasionLifeLimit, 
                prevasionDamageLimit, prevasionCost, bonusOverheatRecovery);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSeptimaDamage, bonusEPUse);

        public override void UpdateEquip(Player player)
        {
            float increase = bonusSeptimaDamage / 100f;
            float increase2 = bonusEPUse / 100f;
            player.GetDamage<SeptimaDamage>() += increase;
            player.GetModPlayer<SeptimaPlayer>().EPUseModifier -= increase2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<QuillOfficerLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            float increase = bonusOverheatRecovery / 100f;
            player.setBonus = SetBonusText.Value;
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.OverheatRecoveryModifier += increase;

            PlayerPrevasion prevasion = player.GetModPlayer<PlayerPrevasion>();
            float limit = prevasionLifeLimit / 100f;
            prevasion.PrevasionLifeLimit = limit;
            // Randomized so there is a chance it works for higher damage too
            prevasion.PrevasionDamageLimit = prevasionDamageLimit;
            prevasion.PrevasionCost = prevasionCost;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBreastplate)
                .AddIngredient(ItemID.Silk, 5)
                .AddIngredient<SpiritualStone>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
