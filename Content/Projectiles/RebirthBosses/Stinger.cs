using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class Stinger : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = (int)TextureAssets.Projectile[ProjectileID.QueenBeeStinger].Size().X;
            Projectile.height = (int)(TextureAssets.Projectile[ProjectileID.QueenBeeStinger].Size().Y);
            Main.projFrames[Projectile.type] = 1;
            Projectile.frame = 0;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300; // 3600
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.QueenBeeStinger}";

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 120);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Poisoned, 120);
        }

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> asset = TextureAssets.Projectile[ProjectileID.QueenBeeStinger];

            Rectangle bounds = asset.Frame();
            Vector2 origin = bounds.Size() / 2;
            Color lightColor = Color.White * 0.15f;
            Main.EntitySpriteDraw(
                asset.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                lightColor, // Lighting.GetColor(Projectile.Center.ToTileCoordinates())
                Projectile.rotation,
                origin,
                2,
                SpriteEffects.None
            );

            Vector2 baseOffset = Vector2.Normalize(Projectile.velocity) * Projectile.height;
            for (int i = 1; i < 5; i++)
            {
                Vector2 offset = baseOffset * i;
                Main.EntitySpriteDraw(
                    asset.Value,
                    Projectile.Center - offset - Main.screenPosition,
                    bounds,
                    lightColor, // Lighting.GetColor(Projectile.Center.ToTileCoordinates())
                    Projectile.rotation,
                    origin,
                    1,
                    SpriteEffects.None
                );
            }
            return base.PreDrawExtras();
        }
    }
}
