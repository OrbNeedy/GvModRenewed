using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Content.Dusts
{
    class PrevasionFeathers : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.alpha = 0;

            int sizeX = 22;
            int sizeY = 26;
            dust.frame = new Rectangle(0, Main.rand.Next(0, 3) * sizeY, sizeX, sizeY);

            dust.customData = Main.rand.Next(0, 1000);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.99f;

            dust.velocity.Y += 0.04f;

            if (dust.customData != null && dust.customData is int count)
            {
                float sinVal = MathF.Sin(count * 0.04f);
                dust.velocity.X += sinVal * 0.1f;

                dust.rotation = dust.velocity.X * -0.2f;

                count++;
                dust.customData = count;
            }

            dust.alpha += 3;

            if (dust.alpha >= 254)
            {
                dust.active = false;
            }

            dust.position += dust.velocity;

            //dust.position += dust.velocity;
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White * 0.3f * (1f - (dust.alpha / 255f));
        }

        public override bool PreDraw(Dust dust)
        {
            return true;
        }
    }
}
