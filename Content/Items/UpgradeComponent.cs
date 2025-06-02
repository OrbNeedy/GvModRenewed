using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Items
{
    public class UpgradeComponent : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Lime;

            Item.maxStack = 999;
        }
    }
}
