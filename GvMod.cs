using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace GvMod
{
	public class GvMod : Mod
	{
        public override void Load()
        {
            Asset<Effect> prevasionShader = this.Assets.Request<Effect>("Effects/PrevasionEffect");

            GameShaders.Misc["Prevasion"] = new MiscShaderData(prevasionShader, "Prevasion");
            base.Load();
        }
	}
}
