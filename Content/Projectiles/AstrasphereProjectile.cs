using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public enum AstraspheredBehavior
    {
        Default, // Follow the owner
        Launch, // Gets sent to the mouse position
        Expand, // Same as the previous, but after getting to the position, disappears and sends the orbits outwards
        Follow // The same behavior as Flashfield, but has higher damage and keeps the orbits
    }
    public class AstrasphereProjectile : ModProjectile
    {
        private int Behavior { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        private int timer = 0;
        private int cycle = 0;
        private static Asset<Texture2D> field;
        private static Asset<Texture2D> extras;
        private float extrasRotation = 0;

        private Rectangle bounds;
        private int extrasFrame = 0;
        private int frame = 0;
        private int frameTimer = 0;
        private bool hideExtras = false;

        public override void SetStaticDefaults()
        {
            extras = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/AstrasphereExtras");
            field = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/AstrasphereProjectile");
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(268);
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 50;
            Projectile.knockBack = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 15;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 100;
            Projectile.ownerHitCheck = false;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            switch (Behavior)
            {
                case (int)AstraspheredBehavior.Launch:
                    Projectile.Size = new Vector2(268);
                    bounds = new Rectangle(0, 0, 360, 362);
                    Projectile.timeLeft += 90;
                    Projectile.netUpdate = true;

                    SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/AstrasphereUse") with
                    {
                        PitchVariance = 0.1f,
                        Volume = 0.75f
                    }, Projectile.Center, StopSound);
                    break;
                default:
                    Projectile.Size = new Vector2(268);
                    bounds = new Rectangle(0, 0, 360, 362);
                    Projectile.netUpdate = true;

                    SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/AstrasphereUse") with
                    {
                        PitchVariance = 0.1f,
                        Volume = 0.75f
                    }, Projectile.Center, StopSound);
                    break;
            }
            timer++;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(timer);
            writer.Write7BitEncodedInt(cycle);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.Read7BitEncodedInt();
            cycle = reader.Read7BitEncodedInt();
            base.ReceiveExtraAI(reader);
        }

        public override void AI()
        {
            switch (Behavior)
            {
                case (int)AstraspheredBehavior.Expand:
                case (int)AstraspheredBehavior.Launch:
                    if (cycle == 0)
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile.Center = Main.LocalPlayer.Center;
                        }
                        if (timer >= 90)
                        {
                            timer = 0;
                            cycle++;
                        }
                        Projectile.netUpdate = true;
                    } else
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            LaunchAstrasphere(Main.MouseWorld);
                        }
                    }
                    break;
                case (int)AstraspheredBehavior.Default:
                default:
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.Center = Main.LocalPlayer.Center;
                        Projectile.netUpdate = true;
                    }
                    break;
            }

            timer++;
            TextureCycles();
        }

        private void LaunchAstrasphere(Vector2 target)
        {
            if (timer <= 1)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(target) *
                    Projectile.Center.Distance(target) / 45;
                Projectile.netUpdate = true;

                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/AstrasphereUse") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.75f
                }, Projectile.Center, StopSound);
            }
            if (timer >= 45)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
        }

        private bool StopSound(ActiveSound soundInstance)
        {
            return Projectile.active;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/AstrasphereEnd") with
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
                case (int)AstraspheredBehavior.Launch:
                default:
                    if (frameTimer >= 4)
                    {
                        frame++;
                        frameTimer = 0;
                        if (frame > 4)
                        {
                            frame = 2;
                            hideExtras = !hideExtras;
                            if (!hideExtras)
                            {
                                extrasFrame++;
                                if (extrasFrame > 1)
                                {
                                    extrasFrame = 0;
                                }
                            }
                        }
                        bounds = new Rectangle(bounds.Width * frame, 0, bounds.Width, bounds.Height);
                    }

                    extrasRotation -= MathHelper.TwoPi / 100;
                    break;
            }
            frameTimer++;
        }

        public override bool? CanCutTiles()
        {
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 ellipsePosition = new Vector2(projHitbox.Left, projHitbox.Top);
            Vector2 ellipseDimentions = new Vector2(projHitbox.Width, projHitbox.Height);
            Vector2 ellipseCenter = ellipsePosition + 0.5f * ellipseDimentions;
            ellipseDimentions *= Projectile.scale;
            float x = 0f;
            float y = 0f;
            if (targetHitbox.Left > ellipseCenter.X)
            {
                x = targetHitbox.Left - ellipseCenter.X;
            }
            else if (targetHitbox.Left + targetHitbox.Width < ellipseCenter.X)
            {
                x = targetHitbox.Left + targetHitbox.Width - ellipseCenter.X;
            }
            if (targetHitbox.Top > ellipseCenter.Y)
            {
                y = targetHitbox.Top - ellipseCenter.Y;
            }
            else if (targetHitbox.Top + targetHitbox.Height < ellipseCenter.Y)
            {
                y = targetHitbox.Top + targetHitbox.Height - ellipseCenter.Y;
            }
            float a = ellipseDimentions.X / 2f;
            float b = ellipseDimentions.Y / 2f;
            return (x * x) / (a * a) + (y * y) / (b * b) <= 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                field.Value, 
                Projectile.Center - Main.screenPosition, 
                bounds, 
                Color.White, 
                0, 
                bounds.Size() * 0.5f, 
                1f, SpriteEffects.None
            );

            if (!hideExtras)
            {
                for (int i = 0; i < 3; i++)
                {
                    float rotationOffset = (MathHelper.TwoPi * i / 3);
                    Vector2 positionOffset = new Vector2(0, 120 * Projectile.scale).
                        RotatedBy(rotationOffset + extrasRotation);

                    Main.EntitySpriteDraw(
                        extras.Value,
                        Projectile.Center - Main.screenPosition + positionOffset,
                        new Rectangle(218 * extrasFrame, 0, 218, 184),
                        Color.White * 0.75f,
                        extrasRotation + rotationOffset - MathHelper.Pi,
                        new Vector2(109, 92),
                        1f, SpriteEffects.None
                    );
                }
            }

            return false;
        }
    }
}
