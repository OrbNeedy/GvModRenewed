using GvMod.Content.Items.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Upgrades
{
    public class ClearUpgrade : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.LightRed;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Item149;
            Item.noMelee = true;
            Item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

        public override bool? UseItem(Player player)
        {
            foreach (Item item in player.inventory)
            {
                if (item.ModItem is DartLeader gun)
                {
                    gun.AddUpgrade(DartLeaderUpgrades.None);
                    break;
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 5)
                .AddIngredient(ItemID.Glass, 5)
                .AddCondition(Condition.PlayerCarriesItem(ModContent.ItemType<DartLeader>()))
                .AddTile(TileID.Anvils)
                .Register();
            base.AddRecipes();
        }
    }
}
