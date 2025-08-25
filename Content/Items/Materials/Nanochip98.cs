using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    public class Nanochip98 : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 5, 25);

            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
