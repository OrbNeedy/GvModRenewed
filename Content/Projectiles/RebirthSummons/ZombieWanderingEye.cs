using GvMod.Common.Players;
using GvMod.Common.Utils;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace GvMod.Content.Projectiles.RebirthSummons
{
    class ZombieWanderingEye : ModProjectile
    {
        public int MaxVisualFrame = 2;
        public int MinVisualFrame = 0;
        public int MaxFrameCounter = 4;
        public bool oldSecondPhase = false;

        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
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

            float speed = 4f;
            float drag = 0.01f;
            bool secondPhase = Projectile.timeLeft < 240;

            TargetNPC = ZombieBat.TargetingSystem(Projectile, TargetNPC, 480, secondPhase);

            if (secondPhase != oldSecondPhase)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 6);
            }

            if (secondPhase)
            {
                MinVisualFrame = 2;
                MaxVisualFrame = 4;
            } else
            {
                MinVisualFrame = 0;
                MaxVisualFrame = 2;
            }

                ControlAnimations();

            bool hasTarget = TargetNPC != -1;
            Vector2 targetPosition = Projectile.Center;

            if (hasTarget)
            {
                targetPosition = Main.npc[TargetNPC].Center;
                speed = 10;
            }

            Vector2 targetVel = Projectile.Center.DirectionTo(targetPosition) * speed;

            if (targetPosition != Projectile.Center)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVel, 0.008f);
            }

            Vector2 newVelocity = ZombieBat.FlyingZombieMovement(Projectile, drag, false, true);

            Projectile.velocity = newVelocity;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            oldSecondPhase = secondPhase;
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.timeLeft < 240)
            {
                modifiers.SourceDamage += 0.25f;
                modifiers.Knockback += 0.5f;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Projectile.timeLeft < 240)
            {
                modifiers.SourceDamage += 1.25f;
                modifiers.Knockback += 0.5f;
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
