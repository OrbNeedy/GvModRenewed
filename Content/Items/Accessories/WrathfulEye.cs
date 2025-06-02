using GvMod.Common.Players;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    public class WrathfulEye : ModItem
    {
        private float epUseModifier = 0.15f;
        private float mainDamageModifier = 0.2f;

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 14;
            Item.accessory = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(mainDamageModifier, epUseModifier);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SeptimaPlayer>().EPUseModifier += epUseModifier;
            player.GetDamage<MainAttackDamage>() += mainDamageModifier;
            base.UpdateAccessory(player, hideVisual);
        }
    }
}
