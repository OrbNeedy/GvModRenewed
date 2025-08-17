using GvMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.GlobalItems
{
    public class GlobalExtractinatorUse : GlobalItem
    {
        public override void ExtractinatorUse(int extractType, int extractinatorBlockType, 
            ref int resultType, ref int resultStack)
        {
            if (extractType == 0 && Main.rand.NextBool(4))
            {
                resultType = ModContent.ItemType<Kripp>();
                resultStack = 1;
            }

            if (extractinatorBlockType == TileID.ChlorophyteExtractinator && extractType == 
                ModContent.ItemType<BlancCells>())
            {
                resultType = ModContent.ItemType<ActinoBlancCrystal>();
                resultStack = 1;
            }
            base.ExtractinatorUse(extractType, extractinatorBlockType, ref resultType, ref resultStack);
        }
    }
}
