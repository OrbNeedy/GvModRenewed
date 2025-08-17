using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    public class ActinoBlancCrystal : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Lime;

            Item.maxStack = Item.CommonMaxStack;
            ItemID.Sets.ShimmerTransformToItem[Item.type] = ModContent.ItemType<BlancCells>();
        }
    }
}
