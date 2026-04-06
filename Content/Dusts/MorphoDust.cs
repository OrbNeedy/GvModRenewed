using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Dusts
{
    class MorphoDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.alpha = 0;

            int sizeX = 22;
            int sizeY = 26;
            dust.frame = new Rectangle(0, Main.rand.Next(0, 3) * sizeY, sizeX, sizeY);

            dust.velocity = new Vector2(0, 1);
            dust.customData = Main.rand.Next(0, 3);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            if (dust.customData != null && dust.customData is int count)
            {
                count++;
                if (count > 2)
                {
                    dust.frame.Y += 26;
                    if (dust.frame.Y >= dust.frame.Height * 4)
                    {
                        dust.frame.Y = 0;
                    }
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
