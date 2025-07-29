using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Items.Materials
{
    public class ScarletGoldFragment : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Lime;

            Item.maxStack = 999;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Gold.ToVector3() * 0.75f * Main.essScale);
        }
    }
}
