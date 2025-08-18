using GvMod.Common.Players;
using GvMod.Content.Items.Armor.Quill;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Armor.ModernQuill
{
    [AutoloadEquip(EquipType.Body)]
    public class ModernQuillBody : ModItem
    {
        // Base 
        private float bonusSeptimaDamage = 16;
        private float bonusEPUse = 16;
        // Set
        private float bonusOverheatRecovery = 15;
        public LocalizedText SetBonusText { get; private set; }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.defense = 16;

            SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(bonusOverheatRecovery);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSeptimaDamage, 
            bonusEPUse);

        public override void UpdateEquip(Player player)
        {
            float bonusDamage = bonusSeptimaDamage / 100f;
            float EPsavings = bonusEPUse / 100f;
            player.GetDamage<SeptimaDamage>() += bonusDamage;
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.EPUseModifier -= EPsavings;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<ModernQuillLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<ResurrectionPlayer>().resurrectionPower += 1f;
            player.GetModPlayer<ResurrectionPlayer>().canResurrect = true;

            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.OverheatRecoveryModifier += bonusOverheatRecovery / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 6)
                .AddIngredient<QuillBody>()
                .AddIngredient<Electromagnet>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
