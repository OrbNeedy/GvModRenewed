using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    [AutoloadEquip(EquipType.Face)]
    public class SealingSpecs : ModItem
    {
        public int bonusMaxEp = 100;
        public float bonusEpUse = 30f;
        public float bonusEpRecovery = 25f;
        public float bonusCooldownRecovery = 20f;
        public float bonusSpRecovery = 20f;
        public float bonusOverheatRecovery = 30f;
        public float damageDecrease = 40f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Red;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusMaxEp, bonusEpUse, 
            bonusEpRecovery, bonusCooldownRecovery, bonusSpRecovery, bonusOverheatRecovery, damageDecrease);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.ModifiedMaxEP += bonusMaxEp;
            adept.EPUseModifier -= bonusEpUse / 100f;
            adept.EPRecoveryModifier += bonusEpRecovery / 100f;
            adept.EPCooldownModifier -= bonusCooldownRecovery / 100f;
            adept.SPRecoveryModifier += bonusSpRecovery / 100f;
            adept.OverheatRecoveryModifier += bonusOverheatRecovery / 100f;

            player.GetDamage<SeptimaDamage>() -= damageDecrease / 100f;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not DynamoEye && equippedItem.ModItem is not CooldownLens
                && equippedItem.ModItem is not RechargeLens && equippedItem.ModItem is not VigorLens 
                && incomingItem.ModItem is not DynamoEye && incomingItem.ModItem is not CooldownLens 
                && incomingItem.ModItem is not RechargeLens && incomingItem.ModItem is not VigorLens;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DynamoEye>()
                .AddIngredient<CooldownLens>()
                .AddIngredient<RechargeLens>()
                .AddIngredient<VigorLens>()
                .AddIngredient<ScarletGoldFragment>(4)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
