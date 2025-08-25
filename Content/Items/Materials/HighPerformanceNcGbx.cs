using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    public class HighPerformanceNcGbx : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 0, 10, 70);

            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
