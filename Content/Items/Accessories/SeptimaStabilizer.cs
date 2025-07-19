using GvMod.Common.Players;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Accessories
{
    public class SeptimaStabilizer : ModItem
    {
        private float epModifier = 5f;
        private float recoveryModifier = 10f;

        public override void SetDefaults()
        {
            Item.accessory = true;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(epModifier, recoveryModifier);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            float epBonus = epModifier / 100f;
            float recoveryBonus = recoveryModifier / 100f;
            player.GetModPlayer<SeptimaPlayer>().EPUseModifier -= epBonus;
            player.GetModPlayer<SeptimaPlayer>().EPRecoveryModifier += recoveryBonus;
        }
    }
}
