using GvMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Upgrades
{
    public class Stage5Upgrade : ModItem
    {
        private int minLevel = 70;
        private int maxLevel = 100;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Lime;

            Item.maxStack = Item.CommonMaxStack;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.UseSound = SoundID.Item15; // 29, 4, 92
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.consumable = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxLevel, minLevel);

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
                return adept.Level >= minLevel && adept.Level < maxLevel;
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();

                return adept.UpgradeLevel(minLevel, maxLevel);
            }
            return null;
        }
    }
}
