using GvMod.Common.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Upgrades
{
    public class Stage1Upgrade : ModItem
    {
        public override void SetDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(2, 5));
            Item.rare = ItemRarityID.Lime;
            Item.maxStack = 999;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.UseSound = SoundID.Item15; // 29, 4, 92
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
                // Can only use it if level is less than 10
                return adept.Level < 10;
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();

                return adept.UpgradeLevel(0, 10);
            }
            return null;
        }
    }
}
