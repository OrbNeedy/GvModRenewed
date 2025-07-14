using GvMod.Common.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Upgrades
{
    public class Stage1Upgrade : ModItem
    {
        private int maxLevel = 20;

        public override void SetDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(2, 5));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Item.rare = ItemRarityID.Lime;
            Item.maxStack = 999;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.UseSound = SoundID.Item15; // 29, 4, 92
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.consumable = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxLevel);

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();

                return adept.Level < maxLevel;
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();

                return adept.UpgradeLevel(0, maxLevel);
            }
            return null;
        }
    }
}
