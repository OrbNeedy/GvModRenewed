using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    public class Electromagnet : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(0, 0, 25, 15);

            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
