using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GvMod.Common.Players;
using GvMod.Common.Utils;
using System;

namespace GvMod.Content.Projectiles.RebirthSummons
{
    class ZombieScorpion : ModProjectile
    {
        public int MaxVisualFrame = 1;
        public int MinVisualFrame = 0;
        public int MaxFrameCounter = 14;

        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 1f;
            MaxVisualFrame = Main.projFrames[Projectile.type] = 4;
            Projectile.frame = 0;

            Projectile.DamageType = ModContent.GetInstance<SeptimaSummonHybrid>();
            Projectile.damage = 6;
            Projectile.knockBack = 1;
            Projectile.penetrate = -1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 14;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 480;
            Projectile.netImportant = true;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            float acceleration = 0.2f;
            float maxSpeed = 4;
            float drag = 0.02f;

            TargetNPC = ZombieBat.TargetingSystem(Projectile, TargetNPC, 480);

            ControlAnimations();

            bool hasTarget = TargetNPC != -1;
            Vector2 targetPosition = Projectile.Center;
            bool groundCollision = GroundCollision(Projectile);

            if (hasTarget)
            {
                targetPosition = Main.npc[TargetNPC].Center;
                //Main.NewText($"Grounded: {groundCollision}  Up: {targetPosition.Y > Projectile.Center.Y + 32}");

                if (groundCollision && targetPosition.Y < Projectile.Center.Y - 48 && 
                    MathF.Abs(targetPosition.X - Projectile.Center.X) <= 32)
                {
                    Projectile.velocity.Y -= 8;
                }
            }
            else
            {
                acceleration = 0.12f;
                maxSpeed = 3;
                targetPosition = Projectile.Center + Projectile.velocity;
            }

            if (targetPosition != Projectile.Center && MathF.Abs(Projectile.velocity.X) < maxSpeed)
            {
                Projectile.velocity.X += Projectile.Center.DirectionTo(targetPosition).X * acceleration;
            }

            Vector2 newVelocity = GroundZombieMovement(Projectile, drag, groundCollision);

            Projectile.velocity = newVelocity;

            if (Projectile.velocity.X != 0)
            {
                MinVisualFrame = 1;
                MaxVisualFrame = 4;
            } else
            {
                MinVisualFrame = 0;
                MaxVisualFrame = 1;
            }
        }

        public static bool GroundCollision(Projectile projectile)
        {
            float halfWidth = projectile.width / 2f;
            float halfHeight = projectile.height / 2f;

            Rectangle down = new Rectangle(
                    (int)(projectile.Center.X - halfWidth), (int)(projectile.Center.Y + (halfHeight * 0.6f)),
                    projectile.width, (int)(halfHeight * 0.4f) + 2
                );

            return Collision.SolidCollision(down.TopLeft(), down.Width, down.Height);
        }

        public static bool IsInsideGround(Projectile projectile)
        {
            float halfWidth = projectile.width / 2f;
            float halfHeight = projectile.height / 2f;

            Rectangle down = new Rectangle(
                    (int)(projectile.Center.X - halfWidth), (int)(projectile.Center.Y + (halfHeight * 0.6f)),
                    projectile.width, (int)(halfHeight * 0.4f)
                );

            return Collision.SolidCollision(down.TopLeft(), down.Width, down.Height);
        }

        public static Vector2 GroundZombieMovement(Projectile projectile, float drag)
        {
            bool groundCollision = GroundCollision(projectile);

            return GroundZombieMovement(projectile, drag, groundCollision);
        }

        public static Vector2 GroundZombieMovement(Projectile projectile, float drag, bool groundCollision)
        {
            bool insideGround = IsInsideGround(projectile);

            Vector2 newVelocity = projectile.velocity;

            newVelocity *= 1f - drag;

            newVelocity.Y += 0.2f;

            float bottomLevel = projectile.Center.Y + (projectile.height / 2f);

            newVelocity += GroundTileCollision(projectile, true, true, true, 0f);

            if (groundCollision)
            {
                // Slow down further when on solid ground
                // newVelocity.X *= 0.95f;
                // If going down, stop
                if (newVelocity.Y > 0)
                {
                    newVelocity.Y = 0f;
                    projectile.position.Y = bottomLevel - projectile.height - 0.001f;
                }
            }

            if (insideGround)
            {
                newVelocity.Y = -0.2f;
            }

            return newVelocity;
        }

        public static Vector2 GroundTileCollision(Projectile projectile, bool collideLeft, bool collideRight,
            bool collideUp, float recoilSpeed = 0.7f)
        {
            Vector2 returnSpeed = Vector2.Zero;

            float halfWidth = projectile.width / 2f;
            float halfHeight = projectile.height / 2f;
            Rectangle left = new Rectangle(
                    (int)(projectile.Center.X - halfWidth) - 10, (int)(projectile.Center.Y - halfHeight),
                    10, projectile.height
                );
            Rectangle right = new Rectangle(
                    (int)(projectile.Center.X + halfWidth), (int)(projectile.Center.Y - halfHeight),
                    10, projectile.height
                );
            Rectangle up = new Rectangle(
                    (int)(projectile.Center.X - halfWidth), (int)(projectile.Center.Y - halfHeight) - 10,
                    projectile.width, 10
                );

            float realRecoilSpeed = 1 + recoilSpeed;
            // Left
            if (collideLeft && Collision.SolidCollision(left.TopLeft(), left.Width, left.Height))
            {
                returnSpeed.X = projectile.velocity.X * -realRecoilSpeed;
            }
            // Right
            if (collideRight && Collision.SolidCollision(right.TopLeft(), right.Width, right.Height))
            {
                returnSpeed.X = projectile.velocity.X * -realRecoilSpeed;
            }
            // Up
            if (collideUp && Collision.SolidCollision(up.TopLeft(), up.Width, up.Height))
            {
                returnSpeed.Y = projectile.velocity.Y * -realRecoilSpeed;
            }

            return returnSpeed;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 60);

            if (Main.myPlayer == Projectile.owner)
            {
                SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
                Tag tag = adept.TaggedNPCs.GetTag(target.whoAmI);
                if (tag.targetIndex == target.whoAmI)
                {
                    adept.TryTriggerTagLifesteal(damageDone);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Poisoned, 30);
        }

        public void ControlAnimations()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= MaxFrameCounter)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }

            if (Projectile.frame >= MaxVisualFrame)
            {
                Projectile.frame = MinVisualFrame;
            }

            if (Projectile.frame < MinVisualFrame)
            {
                Projectile.frame = MinVisualFrame;
            }
        }
    }
}
