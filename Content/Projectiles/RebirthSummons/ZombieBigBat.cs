using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GvMod.Common.Players;
using GvMod.Common.Utils;

namespace GvMod.Content.Projectiles.RebirthSummons
{
    class ZombieBigBat : ModProjectile
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

            float acceleration = 0.24f;
            float maxSpeed = 10;
            float inertia = 0.04f;

            TargetNPC = ZombieBat.TargetingSystem(Projectile, TargetNPC, 96);

            ControlAnimations();

            bool hasTarget = TargetNPC != -1;
            Vector2 targetPosition = Projectile.Center;

            if (hasTarget)
            {
                targetPosition = Main.npc[TargetNPC].Center;
            }
            else
            {
                inertia = 0.006f;
                acceleration = 0.004f;
                maxSpeed = 1;
                if (Main._rand.NextBool(300))
                {
                    Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
                }

                targetPosition = Projectile.Center + Projectile.velocity;
            }

            if (targetPosition != Projectile.Center && Projectile.velocity.Length() < maxSpeed)
            {
                Projectile.velocity += Projectile.Center.DirectionTo(targetPosition) * acceleration;
            }

            Vector2 newVelocity = ZombieBat.FlyingZombieMovement(Projectile, inertia, false, true);

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
