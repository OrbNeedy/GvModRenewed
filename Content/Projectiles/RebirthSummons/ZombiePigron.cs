using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GvMod.Common.Players;
using GvMod.Common.Utils;

namespace GvMod.Content.Projectiles.RebirthSummons
{
    class ZombiePigron : ModProjectile
    {
        public int MaxVisualFrame = 7;
        public int MinVisualFrame = 0;
        public int MaxFrameCounter = 8;
        public float intendedRotation = 0f;

        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int DashTimer { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.scale = 1f;
            Main.projFrames[Projectile.type] = 14;
            MaxVisualFrame = 7;
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

            float acceleration = 0.6f;
            float maxSpeed = 12;
            float drag = 0.085f;

            TargetNPC = ZombieBat.TargetingSystem(Projectile, TargetNPC, 80, false);

            ControlAnimations();

            bool hasTarget = TargetNPC != -1;
            Vector2 targetPosition = Projectile.Center;

            if (hasTarget)
            {
                targetPosition = Main.npc[TargetNPC].Center;
                Vector2 direction = Projectile.Center.DirectionTo(targetPosition);

                if (DashTimer >= 45)
                {
                    if (DashTimer == 45)
                    {
                        Projectile.velocity = direction * 48;
                    }

                    acceleration = 0;
                }
                DashTimer++;

                intendedRotation = Projectile.velocity.ToRotation();
            }
            else
            {
                targetPosition = Projectile.Center + Projectile.velocity;

                acceleration = 0;
                if (DashTimer >= 45)
                {
                    DashTimer++;
                }
                intendedRotation = Projectile.velocity.ToRotation();
            }

            if (DashTimer >= 60)
            {
                DashTimer = 0;
            }

            if (DashTimer >= 45)
            {
                MinVisualFrame = 7;
                MaxVisualFrame = Main.projFrames[Projectile.type];
            } else
            {
                MinVisualFrame = 0;
                MaxVisualFrame = 7;
            }

            if (targetPosition != Projectile.Center && Projectile.velocity.Length() < maxSpeed)
            {
                Projectile.velocity += Projectile.Center.DirectionTo(targetPosition) * acceleration;
            }

            Vector2 newVelocity = ZombieBat.FlyingZombieMovement(Projectile, drag, false, false);

            Projectile.velocity = newVelocity;
            Projectile.rotation = intendedRotation;

            if (Projectile.rotation > MathHelper.PiOver2 || Projectile.rotation < -MathHelper.PiOver2)
            {
                //Main.NewText("Inversed", Color.Red);
                Projectile.spriteDirection = 1;
                Projectile.rotation -= MathHelper.Pi;
            }
            else
            {
                //Main.NewText("Normal", Color.Green);
                Projectile.spriteDirection = -1;
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
            if (Main.myPlayer == Projectile.owner)
            {
                SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
                Tag tag = adept.TaggedNPCs.GetTag(target.whoAmI);
                if (tag.targetIndex == target.whoAmI)
                {
                    adept.TryTriggerTagLifesteal(info.Damage);
                }
            }
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
