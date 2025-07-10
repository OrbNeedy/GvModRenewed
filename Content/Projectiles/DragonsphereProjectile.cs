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
    public class DragonsphereProjectile : ModProjectile
    {
        private int frame = 0;
        private int frameTimer = 0;
        private int extrasFrame = 0;
        private int extrasCounter = 0;
        private bool hideExtras = true;
        private SpriteEffects extrasFlip = SpriteEffects.None;

        private SlotId soundID;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(68);
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 99;
            Projectile.knockBack = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 200;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.ownerHitCheck = false;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/DragonsphereUse") with
            {
                PitchVariance = 0.1f,
                Volume = 0.75f
            }, Projectile.Center);
            base.OnSpawn(source);
        }

        public override void AI()
        {
            ActiveSound soundInstance;
            SoundEngine.TryGetActiveSound(soundID, out soundInstance);

            if (soundInstance == null)
            {
                soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/DragonsphereConstant") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.75f
                }, Projectile.Center, StopSound);
            }
            else
            {
                if (!soundInstance.IsPlaying)
                {
                    soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/DragonsphereConstant") with
                    {
                        PitchVariance = 0.1f,
                        Volume = 0.75f
                    }, Projectile.Center, StopSound);
                }
            }

            TextureCycles();
            base.AI();
        }

        private bool StopSound(ActiveSound soundInstance)
        {
            return Projectile.active;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void TextureCycles()
        {
            if (frameTimer >= 4)
            {
                frame++;
                frameTimer = 0;
                if (frame > 4)
                {
                    frame = 1;
                }

                if (!hideExtras || extrasCounter >= 2)
                {
                    hideExtras = !hideExtras;
                    extrasCounter = 0;

                    switch (Main.rand.Next(0, 3))
                    {
                        case 0:
                            extrasFlip = SpriteEffects.FlipHorizontally;
                            break;
                        case 1:
                            extrasFlip = SpriteEffects.FlipVertically;
                            break;
                        default:
                            extrasFlip = SpriteEffects.None;
                            break;
                    }

                    if (!hideExtras)
                    {
                        extrasFrame++;
                        if (extrasFrame > 1)
                        {
                            extrasFrame = 0;
                        }
                    }
                } else
                {
                    extrasCounter++;
                }
            }

            frameTimer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.
                Request<Texture2D>("GvMod/Content/Projectiles/DragonsphereProjectile");

            Main.EntitySpriteDraw(
                texture.Value,
                Projectile.Center - Main.screenPosition,
                new Rectangle(98 * frame, 0, 98, 98),
                Color.White,
                0,
                new Vector2(42, 42),
                1f, SpriteEffects.None
            );

            if (!hideExtras)
            {
                Asset<Texture2D> extras = ModContent.
                    Request<Texture2D>("GvMod/Content/Projectiles/DragonsphereProjectileExtras");

                Main.EntitySpriteDraw(
                    extras.Value,
                    Projectile.Center - Main.screenPosition,
                    new Rectangle(100 * extrasFrame, 0, 100, 100),
                    Color.White,
                    0,
                    new Vector2(42, 42),
                    1f, extrasFlip
                );
            }

            return false;
        }
    }
}
