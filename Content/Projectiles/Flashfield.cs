using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class Flashfield : ModProjectile
    {
        private Vector2 target = new Vector2(0, 0);
        private int timer = 0;
        private int cycle = 0;
        private Asset<Texture2D> field;
        private Asset<Texture2D> extras;
        private float extrasRotation = 0;

        private Rectangle bounds;
        private int extrasFrame = 0;
        private int frame = 0;
        private int frameTimer = 0;
        private bool hideExtras = false;

        private SlotId soundID;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(242);
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = ModContent.GetInstance<MainAttackDamage>();
            Projectile.damage = 50;
            Projectile.knockBack = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = -1;
            Projectile.ContinuouslyUpdateDamageStats = true;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
            Projectile.ownerHitCheck = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            field = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/Flashfield");
            bounds = new Rectangle(0, 0, 382, 378);
            soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/FlashfieldActive") with
            {
                PitchVariance = 0.1f,
                Volume = 0.25f
            }, Projectile.Center, StopSound);
            frame = Main.rand.Next(0, 4);

            //Main.NewText("Spawn damage: " + Projectile.damage);
        }

        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.Center = Main.LocalPlayer.Center;
                Projectile.netUpdate = true;
            }
            ActiveSound soundInstance;
            SoundEngine.TryGetActiveSound(soundID, out soundInstance);

            if (soundInstance == null)
            {
                soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/FlashfieldUse") with
                {
                    PitchVariance = 0.15f,
                    Volume = 0.25f
                }, Projectile.Center, StopSound);
            } else
            {
                if (!soundInstance.IsPlaying)
                {
                    soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/FlashfieldUse") with
                    {
                        PitchVariance = 0.15f,
                        Volume = 0.25f
                    }, Projectile.Center, StopSound);
                }
            }
            TextureCycles();
        }

        private bool StopSound(ActiveSound soundInstance)
        {
            if (Projectile.active)
            {
                return true;
            } else
            {
                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/FlashfieldEnd") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.25f
                }, Projectile.Center);
                return false;
            }
        }

        private void TextureCycles()
        {
            if (frameTimer >= 2)
            {
                frame++;
                frameTimer = 0;
                if (frame > 3)
                {
                    frame = 0;
                }
                bounds = new Rectangle(bounds.Width * frame, 0, bounds.Width, bounds.Height);
            }

            frameTimer++;
        }

        public override bool? CanCutTiles()
        {
            return false;
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

            return false;
        }
    }
}
