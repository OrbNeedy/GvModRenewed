using GvMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    [AutoloadEquip(EquipType.Face)]
    public class VanishingSpecs : ModItem
    {
        public float prevasionCostDecrease = 20f;
        public float prevasionNoCostChance = 30f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Yellow;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(prevasionNoCostChance,
            prevasionCostDecrease);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PlayerPrevasion>().PrevasionCostModifier -= prevasionCostDecrease / 100f;
            player.GetModPlayer<PlayerPrevasion>().PrevasionCostAvoidanceChance = prevasionNoCostChance /
                100f;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not RecirculatorLens &&
                equippedItem.ModItem is not SeerSpecs && incomingItem.ModItem is not RecirculatorLens &&
                incomingItem.ModItem is not SeerSpecs;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Nanites, 12)
                .AddIngredient(ItemID.Ectoplasm, 4)
                .AddIngredient<RecirculatorLens>()
                .AddIngredient<SeerSpecs>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
