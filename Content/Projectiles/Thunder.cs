using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class Thunder : ModProjectile
    {
        private bool Delay { get => Projectile.ai[0] == 0; }
        private int additionalDuration { get => (int)Projectile.ai[1]; }
        private int timer = 0;
        private int frame = 0;
        private int frameTimer = 0;
        private bool flip = false;
        private bool independentFlip = false;
        private bool darken = false;
        private bool skipDraw = false;
        private bool independentSkipDraw = false;

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 1568;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = ModContent.GetInstance<SeptimaDamage>();
            Projectile.damage = 1;
            Projectile.knockBack = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.ownerHitCheck = false;
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            flip = Main.rand.NextBool();
            independentFlip = Main.rand.NextBool();
            darken = Main.rand.NextBool(3);
            skipDraw = false;
            independentSkipDraw = false;
            if (Delay)
            {
                Projectile.timeLeft += 60;
            }
            if (additionalDuration > 0)
            {
                Projectile.timeLeft += additionalDuration;
            }
            base.OnSpawn(source);
        }
        public override void AI()
        {
            if (Projectile.timeLeft%3 == 0)
            {
                independentFlip = Main.rand.NextBool();
                independentSkipDraw = Main.rand.NextBool(9);
            }

            if (frameTimer > 3)
            {
                flip = Main.rand.NextBool();
                darken = Main.rand.NextBool(3);
                skipDraw = Main.rand.NextBool(11);
                frame++;
                frameTimer = 0;
                if (frame > 2)
                {
                    frame = 1;
                }
            }

            if (Delay)
            {
                if (timer < 62) frame = 0;
                if (timer < 60) frame = 3;
            }
            else
            {
                if (timer < 2)
                {
                    frame = 0; 
                }
            }

            if (Projectile.timeLeft < 2) frame = 3;

            frameTimer++;
            timer++;
            base.AI();
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Delay && timer < 60) return false;
            return timer > 2;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (Delay && timer < 60) return false;
            return timer > 2;
        }

        public override bool CanHitPvp(Player target)
        {
            if (Delay && timer < 60) return false;
            return timer > 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (skipDraw)
            {
                return false;
            }

            // Size: 162 x 228
            // Repeat 7 times
            Texture2D thunder = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/Thunder").Value;
            Texture2D extras = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/ThunderExtras").Value;

            Vector2 size = new Vector2(thunder.Width / 4, thunder.Height);
            //frame = 3;
            Rectangle bounds = new Rectangle((int)size.X * frame, 0, (int)(size.X), (int)size.Y);
            Vector2 origin = new Vector2(size.X, size.Y) * 0.5f;

            // origin = new Vector2(404, 113);
            //Main.NewText("Flipped: " + flip);
            //Main.NewText("Bounds: " + bounds);
            //Main.NewText("Origin: " + origin);
            //Main.NewText("Position: " + (Projectile.Center - Main.screenPosition));
            //Main.NewText("Delay: " + Delay);

            for (int i = -3; i < 4; i++)
            {
                Main.EntitySpriteDraw(
                    thunder,
                    Projectile.Center - Main.screenPosition - new Vector2(0, 224 * i),
                    bounds,
                    darken ? new Color(0.3333f, 0.3333f, 0.3333f) : Color.White,
                    Projectile.rotation,
                    origin,
                    1f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None
                );

                if (!independentSkipDraw && !skipDraw && (frame == 1 || frame == 2))
                {
                    Main.EntitySpriteDraw(
                        extras,
                        Projectile.Center - Main.screenPosition - new Vector2(0, 224 * i),
                        extras.Bounds,
                        darken || frame == 2 ? new Color(0.3333f, 0.3333f, 0.3333f) : Color.White,
                        Projectile.rotation,
                        extras.Bounds.Size() / 2,
                        1f,
                        flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None
                    );
                }
            }

            return false;
        }
    }
}
