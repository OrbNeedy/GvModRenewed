using GvMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    public class SeptimaEmblem : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SorcererEmblem);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

        public override void UpdateEquip(Player player)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<SeptimaDamage>() += 0.15f;
        }
    }
}
