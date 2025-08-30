using GvMod.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    public class MysteriousGemstone : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Lime;

            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
