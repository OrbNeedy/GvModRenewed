using GvMod.Common.GlobalNPCs;
using GvMod.Common.Players;
using GvMod.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace GvMod.Content.Projectiles
{
    class GorgoneiaBeam : ModProjectile
    {
        public float ExtraPotency { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public bool IgnoreResistance { get => Projectile.ai[1] == 1; set => Projectile.ai[1] = value ? 1 : 0; }
        public bool IgnoreCooldown { get => Projectile.ai[2] == 1; set => Projectile.ai[2] = value ? 1 : 0; }

        int frame = 0;
        int frameTimer = 0;
        bool redden = false;
        int gazeSoundTimer = 0;
        SlotId soundID;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16);
            Projectile.light = 1f;
            Projectile.scale = 1f;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 0;
            Projectile.knockBack = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = GorgonGazeBeam.gorgonGazeBeamDuration / 3;
            Projectile.ownerHitCheck = true;
            Projectile.netImportant = true;
        }

        public override string Texture => "GvMod/Content/Projectiles/GorgonGazeBeam";

        public override void OnSpawn(IEntitySource source)
        {
            soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GorgonGaze") with
            {
                PitchVariance = 0.1f
            }, Projectile.Center, StopSound);
        }

        public bool StopSound(ActiveSound sound)
        {
            gazeSoundTimer++;
            return Projectile.active && Projectile.ModProjectile is GorgoneiaBeam;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.Center = Main.LocalPlayer.Center + (Projectile.velocity * 5) +
                    new Vector2(0, -14);
                Projectile.netUpdate = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            ActiveSound soundInstance;
            SoundEngine.TryGetActiveSound(soundID, out soundInstance);

            if (soundInstance == null)
            {
                soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GorgonGaze") with
                {
                    PitchVariance = 0.15f,
                    Volume = 0.25f
                }, Projectile.Center, StopSound);
            }
            else
            {
                if (!soundInstance.IsPlaying)
                {
                    soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GorgonGaze") with
                    {
                        PitchVariance = 0.15f,
                        Volume = 0.25f
                    }, Projectile.Center, StopSound);
                }
            }

            Animation();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void Animation()
        {
            int maxFrames = GorgonGazeBeam.maxGorgonGazeBeamFrames - 2;
            int maxTime = frame < maxFrames ? 2 : 8;
            if (Projectile.timeLeft <= maxTime * 2)
            {
                maxFrames = GorgonGazeBeam.maxGorgonGazeBeamFrames;
            }

            if (Projectile.timeLeft == maxTime * 2)
            {
                frame = maxFrames - 2;
            }

            //Main.NewText("Max Frames: ");

            if (frameTimer++ > maxTime)
            {
                frame++;
                frameTimer = 0;
                redden = !redden;
                if (frame >= maxFrames)
                {
                    frame = 3;
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            // Only hit entities facing towards it
            if (target.direction == Projectile.direction ||
                target.GetGlobalNPC<DebuffNPC>().soulPetrified ||
                (target.GetGlobalNPC<DebuffNPC>().soulPetrificationImmunity > 0 &&
                !IgnoreCooldown))
            {
                return false;
            }
            float lifePercent = (float)target.life / (float)target.lifeMax;

            if (target.boss && lifePercent > 0.5f && !IgnoreResistance)
            {
                return false;
            }

            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            GorgonGazeBeam.PetrifyNPC(target, 1f + ExtraPotency, IgnoreResistance, IgnoreCooldown);
        }

        public override bool CanHitPlayer(Player target)
        {
            // Only hit entities facing towards it
            if (target.direction == Projectile.direction ||
                target.GetModPlayer<PlayerDebuffs>().soulPetrifiedImmunity > 0)
            {
                return false;
            }
            return base.CanHitPlayer(target);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            PlayerDebuffs debuff = target.GetModPlayer<PlayerDebuffs>();
            if (debuff.soulPetrifiedImmunity <= 0)
            {
                target.AddBuff(ModContent.BuffType<SoulPetrification>(), 300);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float maxSeparation = float.DegreesToRadians(5);
            float worthless = 0;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + (Projectile.velocity * 
                GorgonGazeBeam.gazeLength), 5, ref worthless) ||
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + (Projectile.velocity.RotatedBy(maxSeparation) * 
                GorgonGazeBeam.gazeLength), 5, ref worthless) || 
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + (Projectile.velocity.RotatedBy(-maxSeparation) * 
                GorgonGazeBeam.gazeLength), 5, ref worthless) || 
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + (Projectile.velocity.RotatedBy(maxSeparation / 2) * 
                GorgonGazeBeam.gazeLength), 5, ref worthless) || 
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + (Projectile.velocity.RotatedBy(-maxSeparation / 2) * 
                GorgonGazeBeam.gazeLength), 5, ref worthless))
            {
                return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle bounds = GorgonGazeBeam.gazeBeam.Value.Bounds;
            bounds.Height /= GorgonGazeBeam.maxGorgonGazeBeamFrames;
            bounds.Y = frame * bounds.Height;
            Color color = redden ? new Color(1, 0.725f, 0.725f) : Color.White;
            if (frame < 2 || frame >= GorgonGazeBeam.maxGorgonGazeBeamFrames - 2)
            {
                color *= 0.3f;
            }
            else
            {
                color *= 0.6f;
            }

            // Main.instance.PrepareDrawnEntityDrawing(Projectile, shaderID, null);

            Main.EntitySpriteDraw(
                GorgonGazeBeam.gazeBeam.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                color,
                Projectile.rotation,
                new(20, bounds.Size().Y * 0.5f),
                1f,
                SpriteEffects.None
            );

            return false;
        }
    }
}
