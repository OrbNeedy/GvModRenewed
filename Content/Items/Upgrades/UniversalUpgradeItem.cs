using GvMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Upgrades
{
    public class UniversalUpgradeItem : ModItem
    {
        public override void SetDefaults()
        {
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
                // Universal upgrade is universal (Up to a cap)
                return adept.Level < 100 || (adept.Level < 200 && NPC.downedMoonlord);
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();

                // Level cap until moonlord is 100, 200 after Moonlord so septima damage has a chance to get through 
                // other modded content post-moonlord
                if (NPC.downedMoonlord) return adept.UpgradeLevel(0, 200);
                else return adept.UpgradeLevel(0, 100);
            }
            return null;
        }
    }
}
