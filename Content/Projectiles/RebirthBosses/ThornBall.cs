using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using GvMod.Common.Players;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class ThornBall : ModProjectile
    {
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

        public override void SetDefaults()
        {
            Main.instance.LoadProjectile(ProjectileID.ThornBall);
            Projectile.width = (int)TextureAssets.Projectile[ProjectileID.ThornBall].Size().X;
            Projectile.height = (int)TextureAssets.Projectile[ProjectileID.ThornBall].Size().Y;
            Main.projFrames[Projectile.type] = 1;
            Projectile.frame = 0;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.ThornBall}";

        public override void AI()
        {
            //Main.NewText("Thorn Ball: " + Projectile.Center);
            //Main.NewText("Owner: " + Main.player[Projectile.owner].Center);
            Vector2 targetPosition = TargetingSystem();
            Vector2 direction = Projectile.Center.DirectionTo(targetPosition) * 8;

            Projectile.velocity.X = float.Lerp(Projectile.velocity.X, direction.X, 0.01f);
            if (Projectile.velocity.Y < 12)
            {
                Projectile.velocity.Y += 0.08f;
            }

            float recoilSpeed = 0.85f;

            float halfWidth = Projectile.width / 2f;
            float halfHeight = Projectile.height / 2f;
            Rectangle left = new Rectangle(
                    (int)(Projectile.Center.X - halfWidth) - 10, (int)(Projectile.Center.Y - halfHeight),
                    10, Projectile.height
                );
            Rectangle right = new Rectangle(
                    (int)(Projectile.Center.X + halfWidth), (int)(Projectile.Center.Y - halfHeight),
                    10, Projectile.height
                );
            Rectangle up = new Rectangle(
                    (int)(Projectile.Center.X - halfWidth), (int)(Projectile.Center.Y - halfHeight) - 10,
                    Projectile.width, 10
                );
            Rectangle down = new Rectangle(
                    (int)(Projectile.Center.X - halfWidth), (int)(Projectile.Center.Y + halfHeight),
                    Projectile.width, 10
                );

            if (Collision.SolidCollision(left.TopLeft(), left.Width, left.Height) ||
                Collision.SolidCollision(right.TopLeft(), right.Width, right.Height))
            {
                Projectile.velocity.X *= -recoilSpeed / 4f;
            }
            if (Collision.SolidCollision(up.TopLeft(), up.Width, up.Height) ||
                Collision.SolidCollision(down.TopLeft(), down.Width, down.Height))
            {
                Projectile.velocity.Y *= -recoilSpeed;
            }

            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public Vector2 TargetingSystem()
        {
            Vector2 targetPosition = Vector2.Zero;
            float maxDistance = 1600;

            if (TargetNPC == -1)
            {
                NPCTags ownerTags = Main.player[Projectile.owner].GetModPlayer<SeptimaPlayer>().TaggedNPCs;
                foreach (NPC target in Main.ActiveNPCs)
                {
                    if (ValidTarget(target) && (target.Distance(Projectile.Center) <= maxDistance
                        || ownerTags.GetTag(target.whoAmI).targetIndex != -1))
                    {
                        TargetNPC = target.whoAmI;
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
    }
}
