using GvMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.Pulsar
{
    [AutoloadEquip(EquipType.Face)]
    public class PulsarHead : ModItem
    {
        private float bonusSeptimaDamage = 10;
        private float bonusSpecialDamage = 10;
        private int bonusMaxEP = 50;
        public LocalizedText SetBonusText { get; private set; }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Red;
            Item.defense = 14;

            //SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs();
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSeptimaDamage, 
            bonusSpecialDamage, bonusMaxEP);

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SeptimaDamage>() += bonusSeptimaDamage / 100;
            player.GetDamage<SpecialAttackDamage>() += bonusSpecialDamage / 100;
            player.GetModPlayer<SeptimaPlayer>().BaseMaxEP += bonusMaxEP;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return false;
        }

        public override void UpdateArmorSet(Player player)
        {
        }

        public override void AddRecipes()
        {
            /*CreateRecipe()
                .AddTile(TileID.MythrilAnvil)
                .Register();*/
        }
    }
}
