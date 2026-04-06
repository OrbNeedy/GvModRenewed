using GvMod.Common.Players.Skills;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    class SoulSiphonExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = SoulSiphon.MaxSoulSiphonRange;
            Projectile.height = SoulSiphon.MaxSoulSiphonRange;
            Projectile.scale = 1f;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 0;
            Projectile.knockBack = 1;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 6;
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.QueenSlimeSmash}";

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/SoulRelease") with
            {
                Volume = 1f,
                PitchVariance = 0.1f
            }, Projectile.Center);

            Vector2 offset = new Vector2(0, SoulSiphon.MaxSoulSiphonRange);
            for (int i = 0; i < 200; i++)
            {
                offset = offset.RotatedByRandom(MathHelper.TwoPi);
                float dist = Main._rand.NextFloat();
                int dustID = Dust.NewDust(Projectile.Center + (offset * dist), 0, 0, 
                    DustID.Shadowflame, Scale: 2f);
                Main.dust[dustID].noGravity = true;
            }
        }
    }
}
