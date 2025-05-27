
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class OrochiDrone : ModProjectile
    {
        private Vector2 targetPosition = new Vector2(0, 0);
        private int verticalDirection = -1;
        private int horizontalDirection = 1;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 22;
            Projectile.scale = 1f;
            Projectile.light = 0.1f;

            Projectile.DamageType = DamageClass.Default;
            Projectile.damage = 0;
            Projectile.knockBack = 0;

            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override void OnSpawn(IEntitySource source)
        {
            //Main.NewText("Drone spawned");
            targetPosition = Projectile.Center + new Vector2(30 * Projectile.velocity.X, -78);
            verticalDirection = (int)Projectile.velocity.Y;
            horizontalDirection = -(int)Projectile.velocity.X;
            Projectile.velocity = new Vector2(-Projectile.velocity.X, 0);
            base.OnSpawn(source);
        }

        public override void AI()
        {
            //Main.NewText("Drone position: " + Projectile.Center);
            if (Projectile.Center.Distance(targetPosition) >= 20)
            {
                Projectile.Center += Projectile.Center.DirectionTo(targetPosition) * 20;
            } else
            {
                Projectile.Center = targetPosition;
            }

            if (Projectile.timeLeft <= 151)
            {
                if (Projectile.timeLeft >= 109)
                {

                    if ((Projectile.timeLeft - 109) % 7 == 0 || (Projectile.timeLeft - 109) == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GunShot_G") with
                        {
                            PitchVariance = 0.1f
                        }, Projectile.Center);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                            Projectile.velocity * 10, ModContent.ProjectileType<HairDartProjectile>(),
                            Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0],
                            Projectile.ai[1], Projectile.ai[2]);
                    }

                    if (verticalDirection >= 0)
                    {
                        Projectile.velocity = Projectile.velocity.
                            RotatedBy(MathHelper.Pi / 42 * horizontalDirection);
                    } else
                    {
                        Projectile.velocity = Projectile.velocity.
                            RotatedBy(-MathHelper.Pi / 42* horizontalDirection);
                    }
                } else
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        targetPosition = Main.LocalPlayer.Center;
                    }
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            base.AI();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool CanHitPvp(Player target)
        {
            return false;
        }
    }
}
