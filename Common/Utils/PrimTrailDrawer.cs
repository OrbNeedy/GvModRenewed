using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace GvMod.Common.Utils
{
    public struct PrimTrailDrawer
    {
        private Color trailColor = Color.White;
        private static VertexStrip _vertexStrip = new VertexStrip();

        public PrimTrailDrawer(Color trailColor)
        {
            this.trailColor = trailColor;
        }

        public void Draw(Projectile proj)
        {
            MiscShaderData miscShaderData = GameShaders.Misc["LightDisc"];
            miscShaderData.UseSaturation(-2.8f);
            miscShaderData.UseOpacity(2f);
            miscShaderData.Apply();
            _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f);
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        private Color StripColors(float progressOnStrip)
        {
            float num = 1f - progressOnStrip;
            Color result = trailColor * (num * num * num * num) * 0.5f;
            result.A = 0;
            return result;
        }

        private float StripWidth(float progressOnStrip)
        {
            return 8f;
        }
    }
}
