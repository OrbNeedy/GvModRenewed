using GvMod.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    public class RingOfPerfection : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SeptimaPlayer>().perfectionCheck = true;
            player.GetDamage(DamageClass.Generic) += 1000;
            base.UpdateAccessory(player, hideVisual);
        }
    }
}
