using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    public class BlancCells : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Green;

            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
