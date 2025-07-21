using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    public class Kripp : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Green;

            Item.maxStack = 999;
        }
    }
}
