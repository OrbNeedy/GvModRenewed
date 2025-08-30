using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class GrandStrizerProjectile : ModProjectile
    {
        private int Behavior { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        private int timer = 0;
        private int cycle = 0;
        private Asset<Texture2D> field;
        private Asset<Texture2D> extras;
        private float extrasRotation = 0;

        private Rectangle bounds = new Rectangle(0, 0, 404, 296);
        private Vector2 visualScale = new Vector2(0.125f, 0.4f);
        private bool darken = false;
        private int extrasFrame = 0;
        private int frame = 0;
        private int frameTimer = 0;
        private bool hideExtras = false;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(200);
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 250;
            Projectile.knockBack = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 30;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;
            Projectile.ownerHitCheck = false;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            //Main.NewText("Speed: " + Projectile.velocity.Length());
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/LuxcaliburUse") with
            {
                PitchVariance = 0.1f,
                Volume = 0.75f
            }, Projectile.Center);

            switch (Behavior)
            {
                default:
                    field = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/GrandStrizerProjectile");
                    Projectile.netUpdate = true;
                    break;
            }
            extras = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/AstrasphereExtras");
            timer++;
        }

        public override void AI()
        {
            //Main.NewText("Speed: " + Projectile.velocity.Length());
            switch (Behavior)
            {
                default:
                    //Main.NewText("Default Luxcalibur behavior");
                    if (Projectile.velocity.Length() > 0.001f)
                    {
                        Projectile.velocity *= 0.8f;
                    }
                    break;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            timer++;
            TextureCycles();
        }

        public override bool ShouldUpdatePosition()
        {
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/LuxcaliburEnd") with
            {
                PitchVariance = 0.1f,
                Volume = 0.75f
            }, Projectile.Center);
            base.OnKill(timeLeft);
        }

        private void TextureCycles()
        {
            switch (Behavior)
            {
                default:
                    if (frame < 2)
                    {
                        if (frameTimer >= 6)
                        {
                            frame++;
                            if (Projectile.velocity.Length() > 6 && frame >= 2)
                            {
                                frame = 1;
                            }
                            frameTimer = 0;
                        }

                        RecalculateScale();
                    }

                    if (Projectile.timeLeft <= 18)
                    {
                        if (Projectile.timeLeft <= 12)
                        {
                            frame = 4;
                        } else
                        {
                            frame = 3;
                        }
                    }

                    if (Projectile.timeLeft <= 6)
                    {
                        darken = true;
                        if (visualScale.Y > 0)
                        {
                            visualScale.Y -= 0.3333f;
                        }
                    }
                    break;
            }

            bounds.Y = frame * bounds.Height;

            frameTimer++;
        }

        private void RecalculateScale()
        {
            if (visualScale.X < 1)
            {
                visualScale.X += 0.219f;
            }
            else
            {
                visualScale.X = 1;
            }

            if (visualScale.Y < 1)
            {
                visualScale.Y += 0.15f;
            }
            else
            {
                visualScale.Y = 1;
            }
        }
        public override bool? CanCutTiles()
        {
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float targetHealth = MathHelper.Clamp(target.life / target.lifeMax, 0, 1);
            modifiers.SourceDamage += 1 - targetHealth;
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0;
            Vector2 tip = (Projectile.Center + new Vector2(178, 0)).RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 centerBase = (Projectile.Center + new Vector2(-80, 0)).
                RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 base1 = (Projectile.Center + new Vector2(-154, -140)).
                RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 base2 = (Projectile.Center + new Vector2(-154, 140)).
                RotatedBy(Projectile.rotation, Projectile.Center);
            Rectangle boxBounds = new Rectangle((int)(Projectile.Center.X - 250),
                (int)(Projectile.Center.Y - 250), 500, 500);

            // First check the AABB before checking line collision to save time on calculating it
            if (!boxBounds.Intersects(targetHitbox))
            {
                return false;
            }

            bool line1 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), tip, base1,
                10, ref collisionPoint);
            bool line2 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), base1, 
                centerBase, 14, ref collisionPoint);
            bool line3 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), base2,
                centerBase, 14, ref collisionPoint);
            bool line4 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), tip, base2,
                10, ref collisionPoint);
            bool line5 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), tip,
                centerBase, 20, ref collisionPoint);

            return line1 || line2 || line3 || line4 || line5;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                field.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                darken ? new Color(0.5f, 0.5f, 0.5f) * 0.8f : Color.White,
                Projectile.rotation,
                bounds.Size() * 0.5f,
                visualScale, SpriteEffects.None
            );

            return false;
        }
    }
}
