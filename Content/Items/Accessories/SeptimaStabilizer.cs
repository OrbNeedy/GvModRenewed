using GvMod.Common.Players;
using Terraria;
using Terraria.ID;
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

            Item.rare = ItemRarityID.Green;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(epModifier, recoveryModifier);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SeptimaPlayer>().EPUseModifier -= epModifier / 100f;
            player.GetModPlayer<SeptimaPlayer>().EPRecoveryModifier += recoveryModifier / 100f;
        }
    }
}
