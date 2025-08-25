using GvMod.Common.Players;
using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class PrevasionChain : ModItem
    {
        private int prevasionDamageLimit = 25;
        private float prevasionLifeLimit = 0;
        private int prevasionCost = 40;

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.LightRed;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(prevasionLifeLimit,
                prevasionDamageLimit, prevasionCost);

        public override void UpdateEquip(Player player)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PlayerPrevasion prevasion = player.GetModPlayer<PlayerPrevasion>();
            prevasion.PrevasionCost = prevasionCost;
            prevasion.PrevasionDamageLimit = prevasionDamageLimit;
            prevasion.PrevasionLifeLimit = prevasionLifeLimit; // Even if the accessory gives no life limit, set this so there is no cheesing with other similar accessories
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Chain, 2)
                .AddIngredient<SpiritualStone>(10)
                .AddIngredient<BlancCells>(8)
                .AddTile(TileID.Anvils)
                .AddCondition(Condition.DownedSkeletron)
                .Register();
        }
    }
}
