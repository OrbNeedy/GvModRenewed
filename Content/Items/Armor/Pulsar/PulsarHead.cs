using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.Pulsar
{
    [AutoloadEquip(EquipType.Head, EquipType.Face)]
    public class PulsarHead : ModItem
    {
        // Base
        private float bonusSpecialDamage = 15;
        private float bonusCooldownRecovery = 50;
        private int bonusMaxEP = 50;
        // Set
        private float bonusSPrecovery = 20f;
        private float bonusSeptimaDamage = 20;
        private int prevasionDamageLimit = 25;
        private float prevasionLifeLimit = 10f;
        private int prevasionCost = 14;

        public LocalizedText SetBonusText { get; private set; }

        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Red;
            Item.defense = 15;

            SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(prevasionLifeLimit, prevasionDamageLimit, 
                prevasionCost, bonusSPrecovery, bonusSeptimaDamage);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSpecialDamage, bonusMaxEP, bonusCooldownRecovery);

        public override void UpdateEquip(Player player)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.ModifiedMaxEP += bonusMaxEP;
            adept.EPCooldownModifier -= bonusCooldownRecovery / 100f;

            player.GetDamage<SpecialAttackDamage>() += bonusSpecialDamage / 100f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.ModItem is PulsarBody && legs.ModItem is PulsarLegs;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SetBonusText.Value;
            SeptimaType septima = player.GetModPlayer<SeptimaPlayer>().septimaType;
            if (septima != SeptimaType.None)
            {
                player.setBonus += Language.GetText($"Mods.GvMod.PulsarHeadBonus.{septima.ToString()}").Value;
            }
            player.GetModPlayer<SetBonusPlayer>().pulsarUpgrade = true;
            player.GetModPlayer<SetBonusPlayer>().DragonSaviorsBonus = true;

            player.GetModPlayer<SeptimaPlayer>().SPRecoveryModifier += bonusSPrecovery / 100f;
            player.GetDamage<SeptimaDamage>() += bonusSeptimaDamage / 100f;

            PlayerPrevasion prevasion = player.GetModPlayer<PlayerPrevasion>();
            prevasion.PrevasionCost = prevasionCost;
            prevasion.PrevasionDamageLimit = prevasionDamageLimit;
            prevasion.PrevasionLifeLimit = prevasionLifeLimit / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PulsarFragment>(10)
                .AddIngredient(ItemID.LunarBar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
