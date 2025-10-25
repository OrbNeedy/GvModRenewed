using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class WideThunder : ModProjectile
    {
        private int Delay { get => (int)Projectile.ai[0]; }
        private int additionalDuration { get => (int)Projectile.ai[1]; }
        private int timer = 0;
        private int frame = 0;
        private int frameTimer = 0;
        private bool flip = false;
        private bool skipDraw = false;

        public override void SetDefaults()
        {
            Projectile.width = 290;
            Projectile.height = 1568;
            Projectile.light = 1f;
            Projectile.scale = 1f;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 1;
            Projectile.knockBack = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.ownerHitCheck = false;
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            flip = Main.rand.NextBool();
            skipDraw = false;
            if (Delay > 0)
            {
                // Find a sound for a delayed thunder
                Projectile.timeLeft += Delay;
            }
            else
            {
                // Find a sound for a constant thunder 
                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/Crashvolt") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.75f
                }, Projectile.Center);
            }

            if (additionalDuration > 0)
            {
                Projectile.timeLeft += additionalDuration;
            }
            base.OnSpawn(source);
        }
        public override void AI()
        {
            if (frameTimer > 3)
            {
                flip = Main.rand.NextBool();
                skipDraw = Main.rand.NextBool(11);
                frame++;
                frameTimer = 0;
                if (frame > 3)
                {
                    frame = 1;
                }
            }

            if (Delay > 0)
            {
                if (timer < Delay + 2) frame = 0;
                if (timer < Delay) frame = 4;
            }
            else
            {
                if (timer < 2)
                {
                    frame = 0;
                }
            }

            if (Projectile.timeLeft < 2) frame = 4;

            frameTimer++;
            timer++;
            base.AI();
        }

        public override bool? CanHitNPC(NPC target)
        {
            if ((Delay > 0 && timer < Delay) || timer < 2) return false;
            return base.CanHitNPC(target);
        }

        public override bool CanHitPlayer(Player target)
        {
            if ((Delay > 0 && timer < Delay) || timer < 2) return false;
            return base.CanHitPlayer(target);
        }

        public override bool CanHitPvp(Player target)
        {
            if ((Delay > 0 && timer < Delay) || timer < 2) return false;
            return base.CanHitPvp(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (skipDraw)
            {
                return false;
            }

            // Size: 322 x 228
            // Repeat 7 times
            Texture2D thunder = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/WideThunder").Value;

            Vector2 size = new Vector2(thunder.Width / 5, thunder.Height);
            Rectangle bounds = new Rectangle((int)size.X * frame, 0, (int)(size.X), (int)size.Y);
            Vector2 origin = new Vector2(size.X, size.Y) * 0.5f;

            for (int i = -3; i < 4; i++)
            {
                Main.EntitySpriteDraw(
                    thunder,
                    Projectile.Center - Main.screenPosition - new Vector2(0, 224 * i),
                    bounds,
                    Color.White,
                    Projectile.rotation,
                    origin,
                    1f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None
                );
            }

            return false;
        }
    }
}
