using GvMod.Common.Players;
using GvMod.Content.Items.Accessories.Pendants;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories.Lenses
{
    [AutoloadEquip(EquipType.Face)]
    public class SoulmateSight : ModItem
    {
        public float bonusDamage = 15f;
        public float anthemBonusDamage = 17f;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Red;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(bonusDamage, anthemBonusDamage);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ResurrectionPlayer resurrection = player.GetModPlayer<ResurrectionPlayer>();
            
            player.GetDamage<SeptimaDamage>() += bonusDamage / 100f;

            if (resurrection.resurrected)
            {
                player.GetDamage<SeptimaDamage>() += anthemBonusDamage / 100f;
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return equippedItem.ModItem is not LoversLens && equippedItem.ModItem is not AdmirersEye 
                && equippedItem.ModItem is not SweepersLens && incomingItem.ModItem is not LoversLens 
                && incomingItem.ModItem is not AdmirersEye && incomingItem.ModItem is not SweepersLens;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LoversLens>()
                .AddIngredient<AdmirersEye>()
                .AddIngredient<SweepersLens>()
                .AddIngredient<ScarletGoldFragment>(4)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
