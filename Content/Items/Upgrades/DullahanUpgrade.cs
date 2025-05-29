using GvMod.Content.Items.Weapons;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace GvMod.Content.Items.Upgrades
{
    public class DullahanUpgrade : ModItem
    {
        public static int additionalDamage = 49;
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

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(additionalDamage);

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
                    gun.AddUpgrade(DartLeaderUpgrades.Dullahan);
                    break;
                }
            }
            return true;
        }
    }
}
