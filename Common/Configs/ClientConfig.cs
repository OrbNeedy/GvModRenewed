using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace GvMod.Common.Configs
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("SeptimaSelection")]
        [DefaultValue(300f)]
        [Range(0f, 1600f)]
        public float MrPlaguesButtonOffsetY;
    }
}
