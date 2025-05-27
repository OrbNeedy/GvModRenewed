using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Content.Items.Weapons;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Upgrades
{
    public class VasukiUpgrade : ModItem
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
                    gun.AddUpgrade(DartLeaderUpgrades.Vasuki);
                    break;
                }
            }
            return true;
        }
    }
}
