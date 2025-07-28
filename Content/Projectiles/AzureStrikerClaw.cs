using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class AzureStrikerClaw : ModProjectile
    {
        Vector2 lineStart = Vector2.Zero;
        PrimTrailDrawer trailDrawer = new(new Color(77, 242, 229));

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3; 
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 1f;
            Projectile.light = 0.175f;

            Projectile.DamageType = ModContent.GetInstance<MainAttackDamage>();
            Projectile.damage = 25;
            Projectile.knockBack = 5;
            Projectile.penetrate = -1;

            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 62;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.ownerHitCheck = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            lineStart = Projectile.Center;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 60)
            {
                // Play sound
                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/Unknown") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.75f
                }, Projectile.Center);
                Projectile.Center += Vector2.Normalize(Projectile.velocity) * 800;
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.TwoPi;
            }

            if (Projectile.timeLeft == 3)
            {
                // Play sound
                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/Unknown") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.75f
                }, Projectile.Center);
                lineStart = Projectile.Center;
                Vector2 direction = Projectile.Center.DirectionTo(Main.player[Projectile.owner].Center);
                Projectile.Center += direction * Projectile.Center.Distance(
                    Main.player[Projectile.owner].Center);
                Projectile.velocity = direction;
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.TwoPi;
            }

            if (Projectile.timeLeft < 61 && Projectile.timeLeft > 2)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.15f);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly || target.immortal || !target.active) return;

            SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
            if (target.active && target.life > 0 && !target.immortal && !target.friendly &&
                adept.septimaType != SeptimaType.None)
            {
                // Instant max tag
                adept.TaggedNPCs.AddTag(target.whoAmI, 900, 3);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft < 60 && Projectile.timeLeft > 3)
            {
                return base.Colliding(projHitbox, targetHitbox);
            }

            float point = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, 
                Projectile.Center, 10, ref point);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            trailDrawer.Draw(Projectile);
            return base.PreDraw(ref lightColor);
        }
    }
}
