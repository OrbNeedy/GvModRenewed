using GvMod.Common.Players;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles.RebirthSummons
{
    class ZombieBat : ModProjectile
    {
        public int MaxVisualFrame = 1;
        public int MinVisualFrame = 0;
        public int MaxFrameCounter = 15;

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

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            float acceleration = 0.18f;
            float maxSpeed = 8;
            float inertia = 0.01f;

            TargetNPC = TargetingSystem(Projectile, TargetNPC, 80);

            ControlAnimations();

            bool hasTarget = TargetNPC != -1;
            Vector2 targetPosition = Projectile.Center;

            if (hasTarget)
            {
                targetPosition = Main.npc[TargetNPC].Center;
            }
            else
            {
                inertia = 0.02f;
                acceleration = 0.02f;
                maxSpeed = 2;
                if (Main._rand.NextBool(600))
                {
                    Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
                }

                targetPosition = Projectile.Center + Projectile.velocity;
            }

            if (targetPosition != Projectile.Center && Projectile.velocity.Length() < maxSpeed)
            {
                Projectile.velocity += Projectile.Center.DirectionTo(targetPosition) * acceleration;
            }

            Vector2 newVelocity = FlyingZombieMovement(Projectile, inertia, false, true);

            Projectile.velocity = newVelocity;

            if (Projectile.velocity.X < 0)
            {
                Projectile.spriteDirection = 1;
            }
            else
            {
                Projectile.spriteDirection = -1;
            }
        }

        public static Vector2 FlyingZombieMovement(Projectile projectile, float drag = 0.02f, 
            bool gravity = false, bool tileCollision = false)
        {
            Vector2 newVelocity = projectile.velocity;

            newVelocity *= 1f - drag;

            if (gravity)
            {
                newVelocity.Y += 0.2f;

                int extraWidth = 32;
                if (Collision.SolidTiles(projectile.Center - (projectile.Size / 2f) - new Vector2(extraWidth, 0), 
                    projectile.width + (extraWidth * 2), 96, true))
                {
                    newVelocity.Y -= 0.24f;
                }
            }

            if (tileCollision)
            {
                newVelocity += TileCollision(projectile, true, true, true, true);
            }

            return newVelocity;
        }

        public static Vector2 TileCollision(Projectile projectile, bool collideLeft, bool collideRight, 
            bool collideUp, bool collideDown, float recoilSpeed = 0.7f)
        {
            Vector2 returnSpeed = Vector2.Zero;

            float halfWidth = projectile.width / 2f;
            float halfHeight = projectile.height / 2f;
            Rectangle left = new Rectangle(
                    (int)(projectile.Center.X - halfWidth), (int)(projectile.Center.Y - halfHeight),
                    (int)(projectile.width * 0.2f), projectile.height
                );
            Rectangle right = new Rectangle(
                    (int)(projectile.Center.X + (int)(projectile.width * 0.8f)), (int)(projectile.Center.Y - halfHeight),
                    (int)(projectile.width * 0.2f), projectile.height
                );
            Rectangle up = new Rectangle(
                    (int)(projectile.Center.X - halfWidth), (int)(projectile.Center.Y - halfHeight),
                    projectile.width, (int)(projectile.height * 0.2f)
                );
            Rectangle down = new Rectangle(
                    (int)(projectile.Center.X - halfWidth), (int)(projectile.Center.Y + (int)(halfHeight * 0.8f)),
                    projectile.width, (int)(projectile.height * 0.2f)
                );

            float realRecoilSpeed = 1f + recoilSpeed;
            // Left
            if (collideLeft && Collision.SolidCollision(left.TopLeft(), left.Width, left.Height) && 
                projectile.velocity.X < 0)
            {
                returnSpeed.X = projectile.velocity.X * -realRecoilSpeed;
            }
            // Right
            if (collideRight && Collision.SolidCollision(right.TopLeft(), right.Width, right.Height) &&
                projectile.velocity.X > 0)
            {
                returnSpeed.X = projectile.velocity.X * -realRecoilSpeed;
            }
            // Up
            if (collideUp && Collision.SolidCollision(up.TopLeft(), up.Width, up.Height) &&
                projectile.velocity.Y < 0)
            {
                returnSpeed.Y = projectile.velocity.Y * -realRecoilSpeed;
            }
            // Down
            if (collideDown && Collision.SolidCollision(down.TopLeft(), down.Width, down.Height) &&
                projectile.velocity.Y > 0)
            {
                returnSpeed.Y = projectile.velocity.Y * -realRecoilSpeed;
            }

            return returnSpeed;
        }

        public static int TargetingSystem(Projectile projectile, int previousTarget, float maxDistance, 
            bool allowDetargeting = true)
        {
            int newtarget = previousTarget;

            if (newtarget == -1)
            {
                NPCTags ownerTags = Main.player[projectile.owner].GetModPlayer<SeptimaPlayer>().TaggedNPCs;
                foreach (NPC target in Main.ActiveNPCs)
                {
                    if (ValidTarget(target))
                    {
                        float targetSizeMax = (target.width + target.height) / 2f;

                        if (target.Distance(projectile.Center) <= maxDistance + targetSizeMax)
                        {
                            newtarget = target.whoAmI;
                            break;
                        }
                    }
                }
            }
            else
            {
                NPCTags ownerTags = Main.player[projectile.owner].GetModPlayer<SeptimaPlayer>().TaggedNPCs;

                if (!ValidTarget(Main.npc[newtarget]) || (ownerTags.GetTag(newtarget).targetIndex == -1 &&
                    Main.npc[newtarget].Distance(projectile.Center) > maxDistance && allowDetargeting))
                {
                    newtarget = -1;
                }
            }

            return newtarget;
        }

        public static int TargetingSystem(Projectile projectile, int previousTarget)
        {
            int newtarget = previousTarget;

            if (newtarget == -1)
            {
                NPCTags ownerTags = Main.player[projectile.owner].GetModPlayer<SeptimaPlayer>().TaggedNPCs;
                foreach (NPC target in Main.ActiveNPCs)
                {
                    if (ValidTarget(target) && ownerTags.GetTag(target.whoAmI).targetIndex != -1)
                    {
                        newtarget = target.whoAmI;
                        break;
                    }
                }

                if (Main.myPlayer == projectile.owner)
                {
                    projectile.netUpdate = true;
                }
            }
            else
            {
                NPCTags ownerTags = Main.player[projectile.owner].GetModPlayer<SeptimaPlayer>().TaggedNPCs;

                if (!ValidTarget(Main.npc[newtarget]))
                {
                    newtarget = -1;

                    if (Main.myPlayer == projectile.owner)
                    {
                        projectile.netUpdate = true;
                    }
                }
            }

            return newtarget;
            //Main.NewText("Target position after targeting: " + targetPosition);
            //Main.NewText("Projectile position: " + Projectile.Center + "\n\n");
        }

        public static bool ValidTarget(NPC target)
        {
            if (!target.friendly && target.life > 0 && target.type != NPCID.TargetDummy &&
                target.CanBeChasedBy() && target.active)
            {
                return true;
            }
            return false;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
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
        }
    }
}
