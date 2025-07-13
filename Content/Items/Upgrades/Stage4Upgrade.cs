using GvMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Upgrades
{
    public class Stage4Upgrade : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Lime;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;

            Item.maxStack = 999;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.UseSound = SoundID.Item15; // 29, 4, 92
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
                return adept.Level >= 50 && adept.Level < 90;
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();

                return adept.UpgradeLevel(50, 90);
            }
            return null;
        }
    }
}
