using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GvMod.Common.Players;
using GvMod.Common.Utils;
using Terraria.DataStructures;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class SkullProjectile : ModProjectile
    {
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public float Speed { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }

        public override void SetDefaults()
        {
            Projectile.width = (int)TextureAssets.Projectile[ProjectileID.Skull].Size().X;
            Projectile.height = (int)(TextureAssets.Projectile[ProjectileID.Skull].Size().Y / 3);
            Main.projFrames[Projectile.type] = 3;
            Projectile.frame = 0;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Skull}";

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            int currentTarget = TargetNPC;
            Vector2 targetPosition = TargetingSystem(ref currentTarget);
            Vector2 targetVelocity = Projectile.Center.DirectionTo(targetPosition);

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 
               0.08f);
            Projectile.velocity.Normalize();
            Projectile.velocity *= Speed;

            Projectile.rotation = Projectile.velocity.ToRotation();

            TargetNPC = currentTarget;

            //if (Projectile.velocity.X > 0) Projectile.spriteDirection = -1;

            if (Speed < 10) Speed += 0.06f;
        }

        public Vector2 TargetingSystem(ref int targetNPC)
        {
            Vector2 targetPosition = Vector2.Zero;
            float maxDistance = 1600;

            if (targetNPC == -1)
            {
                NPCTags ownerTags = Main.player[Projectile.owner].GetModPlayer<SeptimaPlayer>().TaggedNPCs;
                foreach (NPC target in Main.ActiveNPCs)
                {
                    if (ValidTarget(target) && (target.Distance(Projectile.Center) <= maxDistance
                        || ownerTags.GetTag(target.whoAmI).targetIndex != -1))
                    {
                        targetNPC = target.whoAmI;
                        targetPosition = target.Center;
                        break;
                    }
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    targetPosition = Main.MouseWorld;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                NPCTags ownerTags = Main.player[Projectile.owner].GetModPlayer<SeptimaPlayer>().TaggedNPCs;

                if (!ValidTarget(Main.npc[TargetNPC]) || (ownerTags.GetTag(TargetNPC).targetIndex == -1 &&
                    Main.npc[TargetNPC].Distance(Projectile.Center) > maxDistance))
                {
                    TargetNPC = -1;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        targetPosition = Main.MouseWorld;
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    targetPosition = Main.npc[TargetNPC].Center;
                }
            }

            return targetPosition;
            //Main.NewText("Target position after targeting: " + targetPosition);
            //Main.NewText("Projectile position: " + Projectile.Center + "\n\n");
        }

        public bool ValidTarget(NPC target)
        {
            if (!target.friendly && target.life > 0 && target.type != NPCID.TargetDummy &&
                target.CanBeChasedBy() && target.active)
            {
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return base.PreDraw(ref lightColor);
        }
    }
}
