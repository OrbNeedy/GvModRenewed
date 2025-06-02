using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace GvMod.Common.Configs
{
    public class GameplayConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Resurrection")]
        [DefaultValue(true)]
        public bool BossResurrection;
    }
}
