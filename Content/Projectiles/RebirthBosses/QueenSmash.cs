using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class QueenSmash : ModProjectile
    {
        public override void SetDefaults()
        {
            Main.instance.LoadProjectile(ProjectileID.ThornBall);
            Projectile.width = (int)TextureAssets.Projectile[ProjectileID.QueenSlimeSmash].Size().X;
            Projectile.height = (int)TextureAssets.Projectile[ProjectileID.QueenSlimeSmash].Size().Y;
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
            Projectile.timeLeft = 40;
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.QueenSlimeSmash}";

        public override void AI()
        {
            Projectile.width += 12;
            Projectile.height += 12;
            Projectile.Center -= new Vector2(6, 6);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
    }
}
