using GvMod.Content.Items.Weapons;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Upgrades
{
    public class CapacityUpgrade : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Green;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Item149;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                foreach (Item item in player.inventory)
                {
                    if (item.ModItem is DartLeader)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                foreach (Item item in player.inventory)
                {
                    if (item.ModItem is DartLeader gun)
                    {
                        return gun.IncreaseCapacity();
                    }
                }
                return null;
            }
            return null;
        }
    }
}
