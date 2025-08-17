using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Dusts
{
    public class LumenDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.alpha = 0;

            int sizeX = 28;
            int sizeY = 32;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * sizeY, sizeX, sizeY);

            dust.velocity = new Vector2(0, 1);
            dust.customData = Main.rand.Next(3);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            if (dust.customData != null && dust.customData is int count)
            {
                if (count > 3)
                {
                    dust.frame = new Rectangle(0, Main.rand.Next(3) * 32, 28, 32);
                }

                count++;
                if (count > 5)
                {
                    count = 0;
                }
                dust.customData = count;
            }

            dust.alpha++;

            if (dust.alpha > 180)
            {
                dust.active = false;
            }
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            if (dust.customData != null && dust.customData is int count)
            {
                if (count > 3) return new Color(1, 1, 1, 0);
            }
            return Color.White * 0.5f;
        }

        public override bool PreDraw(Dust dust)
        {
            return true;
        }
    }
}
