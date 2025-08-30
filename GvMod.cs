using GvMod.Content.Items;
using GvMod.Content.Items.Accessories;
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
            Asset<Effect> morvoltShader = Assets.Request<Effect>("Effects/RainbowEffect");

            GameShaders.Misc["GvMod:Rainbow"] = new MiscShaderData(morvoltShader, "MorvoltWings");

            GameShaders.Armor.BindShader(ModContent.ItemType<PowerfulDye>(), 
                new ArmorShaderData(morvoltShader, "MorvoltWings"));
            base.Load();
        }
	}
}
