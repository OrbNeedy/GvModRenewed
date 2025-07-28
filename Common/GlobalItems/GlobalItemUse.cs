using GvMod.Common.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalItems
{
    public class GlobalItemUse : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            SeptimaPlayer adept = player.GetModPlayer<SeptimaPlayer>();
            if (adept.septimaType != Players.Sevenths.SeptimaType.None)
            {
                adept.septima.ItemUse(player, adept, item);
            }
            return base.UseItem(item, player);
        }
    }
}
