using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using Terraria.ModLoader;

namespace GvMod.Common.Systems
{
    public class SeptimaLoadingSystem : ModSystem
    {
        public override void Load()
        {
            foreach (Septima septima in SeptimaPlayer._templateSeptimas)
            {
                septima.LoadSeptima(Mod);
            }
        }
    }
}
