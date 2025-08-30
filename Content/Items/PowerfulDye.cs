using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items
{
    public class PowerfulDye : ModItem
    {
        public override void SetDefaults()
        {
            int dye = Item.dye;

            Item.CloneDefaults(ItemID.AcidDye);

            Item.dye = dye;
        }
    }
}
