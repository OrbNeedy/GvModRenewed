using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GvMod.Common.Players;
using GvMod.Common.Utils;

namespace GvMod.Content.Projectiles.RebirthSummons
{
    class ZombieHellBat : ModProjectile
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
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.scale = 1f;
            Projectile.light = 0.6f;
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
            float drag = 0.04f;

            TargetNPC = ZombieBat.TargetingSystem(Projectile, TargetNPC, 80);

            ControlAnimations();

            bool hasTarget = TargetNPC != -1;
            Vector2 targetPosition = Projectile.Center;

            if (hasTarget)
            {
                targetPosition = Main.npc[TargetNPC].Center;
            }
            else
            {
                drag = 0.04f;
                acceleration = 0.12f;
                maxSpeed = 5;
                targetPosition = Projectile.Center + Projectile.velocity;
            }

            if (targetPosition != Projectile.Center && Projectile.velocity.Length() < maxSpeed)
            {
                Projectile.velocity += Projectile.Center.DirectionTo(targetPosition) * acceleration;
            }

            Vector2 newVelocity = ZombieBat.FlyingZombieMovement(Projectile, drag, false, true);

            Projectile.velocity = newVelocity;

            if (Main._rand.NextBool(4))
            {
                for (int i = 0; i < Main._rand.Next(1, 4); i++)
                {
                    Dust.NewDust(Projectile.Center - (Projectile.Size / 2), Projectile.width,
                        Projectile.height, DustID.Torch);
                }
            }

            if (Projectile.velocity.X < 0)
            {
                Projectile.spriteDirection = 1;
            }
            else
            {
                Projectile.spriteDirection = -1;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.hardMode)
            {
                target.AddBuff(BuffID.OnFire3, 150);
            } else
            {
                target.AddBuff(BuffID.OnFire, 150);
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
            if (Main.hardMode)
            {
                target.AddBuff(BuffID.OnFire3, 90);
            }
            else
            {
                target.AddBuff(BuffID.OnFire, 90);
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
