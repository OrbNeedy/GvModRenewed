using GvMod.Content.Projectiles.Hooks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    class RebirthHitbox : ModProjectile
    {
        public int HookID { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(50);
            //Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.damage = 150;
            Projectile.knockBack = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 4;
            Projectile.ownerHitCheck = true;
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.QueenSlimeSmash}";

        public override void AI()
        {
            if (HookID >= 0)
            {
                Projectile hook = Main.projectile[HookID];
                if (hook.owner == Projectile.owner && hook.ModProjectile is RebirthHook && 
                    hook.active)
                {
                    Projectile.Size = new Vector2(16);
                    Projectile.Center = hook.Center;
                    Projectile.timeLeft++;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (HookID >= 0)
            {
                Projectile hook = Main.projectile[HookID];
                if (hook.owner == Projectile.owner && hook.ModProjectile is RebirthHook &&
                    hook.active)
                {
                    return hook.velocity.Length() > 1;
                }
            }
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (HookID >= 0)
            {
                Projectile hook = Main.projectile[HookID];
                if (hook.owner == Projectile.owner && hook.ModProjectile is RebirthHook &&
                    hook.active)
                {
                    return hook.velocity.Length() > 1;
                }
            }
            return false;
        }
    }
}
