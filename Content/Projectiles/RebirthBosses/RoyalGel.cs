using GvMod.Common.Players;
using GvMod.Common.Utils;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class RoyalGel : ModProjectile
    {
        public int BounceCount { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }

        public override void SetDefaults()
        {
            Main.instance.LoadProjectile(ProjectileID.ThornBall);
            Projectile.width = (int)TextureAssets.Projectile[ProjectileID.QueenSlimeGelAttack].Size().X;
            Projectile.height = (int)TextureAssets.Projectile[ProjectileID.QueenSlimeGelAttack].Size().Y;
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

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.QueenSlimeGelAttack}";

        public override void AI()
        {
            //Main.NewText("Thorn Ball: " + Projectile.Center);
            //Main.NewText("Owner: " + Main.player[Projectile.owner].Center);
            if (Projectile.velocity.Y < 14)
            {
                Projectile.velocity.Y += 0.1f;
            }

            float recoilSpeed = 1f;

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

            bool leftCollision = Collision.SolidCollision(left.TopLeft(), left.Width, left.Height);
            bool rightCollision = Collision.SolidCollision(right.TopLeft(), right.Width, right.Height);
            bool upCollision = Collision.SolidCollision(up.TopLeft(), up.Width, up.Height);
            bool downCollision = Collision.SolidCollision(down.TopLeft(), down.Width, down.Height);

            if (leftCollision || rightCollision)
            {
                Projectile.velocity.X *= -recoilSpeed;
            }
            if (upCollision || downCollision)
            {
                Projectile.velocity.Y *= -recoilSpeed;
            }

            if (leftCollision || rightCollision || upCollision || downCollision)
            {
                BounceCount++;
                if (BounceCount > 2)
                {
                    Projectile.timeLeft = -1;
                    Projectile.active = false;
                }
            }

            Projectile.rotation += Projectile.velocity.X < 0 ? -0.2f : 0.2f;
        }
    }
}
