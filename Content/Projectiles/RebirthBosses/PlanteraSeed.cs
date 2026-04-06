using GvMod.Common.Players;
using GvMod.Common.Utils;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class PlanteraSeed : ModProjectile
    {
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public bool Poison { get => Projectile.ai[1] == 1; set => Projectile.ai[1] = value ? 1 : 0; }

        public override void SetDefaults()
        {
            Main.instance.LoadProjectile(ProjectileID.SeedPlantera);
            Main.instance.LoadProjectile(ProjectileID.PoisonSeedPlantera);
            Projectile.width = (int)TextureAssets.Projectile[ProjectileID.SeedPlantera].Size().X;
            Projectile.height = (int)(TextureAssets.Projectile[ProjectileID.SeedPlantera].Size().Y / 2);
            Main.projFrames[Projectile.type] = 2;
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

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.SeedPlantera}";

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
               0.2f);
            Projectile.velocity.Normalize();
            Projectile.velocity *= 12;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            TargetNPC = currentTarget;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Poison)
            {
                target.AddBuff(BuffID.Poisoned, 150);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Poison)
            {
                target.AddBuff(BuffID.Poisoned, 150);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int seedID = ProjectileID.SeedPlantera;
            if (Poison) seedID = ProjectileID.PoisonSeedPlantera;
            Asset<Texture2D> seed = TextureAssets.Projectile[seedID];
            Main.EntitySpriteDraw(
                seed.Value, 
                Projectile.Center - Main.screenPosition, 
                seed.Frame(1, 2, 0, Projectile.frame), 
                lightColor, 
                Projectile.rotation, 
                new Vector2(Projectile.width / 2, Projectile.height / 4), 
                1, 
                SpriteEffects.None
                );

            return false;
        }
    }
}
