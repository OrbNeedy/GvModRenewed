using GvMod.Common.Players;
using GvMod.Common.Utils;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace GvMod.Content.Projectiles.RebirthSummons
{
    class ZombieFlyingEye : ModProjectile
    {
        public int MaxVisualFrame = 1;
        public int MinVisualFrame = 0;
        public int MaxFrameCounter = 4;

        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.scale = 1f;
            MaxVisualFrame = Main.projFrames[Projectile.type] = 2;
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

            float speed = 2f;
            float drag = 0.01f;

            TargetNPC = ZombieBat.TargetingSystem(Projectile, TargetNPC, 80, false);

            ControlAnimations();

            bool hasTarget = TargetNPC != -1;
            Vector2 targetPosition = Projectile.Center;

            if (hasTarget)
            {
                targetPosition = Main.npc[TargetNPC].Center;
                speed = 8;
            }

            Vector2 targetVel = Projectile.Center.DirectionTo(targetPosition) * speed;

            if (targetPosition != Projectile.Center)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVel, 0.006f);
            }

            Vector2 newVelocity = ZombieBat.FlyingZombieMovement(Projectile, drag, false, true);

            Projectile.velocity = newVelocity;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
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
