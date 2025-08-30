using GvMod.Common.Players;
using GvMod.Common.Systems;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    [AutoloadEquip(EquipType.Face)]
    public class VigorLens : ModItem
    {
        private float bonusSPIncrease = 25f;
        private float EPRecoveryPenalty = 20f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.LightRed;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusSPIncrease, 
            EPRecoveryPenalty);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            adept.SPRecoveryModifier += bonusSPIncrease / 100f;
            adept.EPRecoveryModifier -= EPRecoveryPenalty / 100f;
            adept.EPCooldownModifier += EPRecoveryPenalty / 100f;
            adept.OverheatRecoveryModifier -= EPRecoveryPenalty / 100f;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not SealingSpecs && incomingItem.ModItem is not SealingSpecs;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddIngredient<SpiritualStone>(8)
                .AddIngredient(ItemID.Bone, 12)
                .AddRecipeGroup(RecipeGroups.IronBar.ToString(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
