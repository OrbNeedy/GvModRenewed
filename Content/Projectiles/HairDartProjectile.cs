using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class HairDartProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.scale = 1f;
            Projectile.light = 0.1f;

            Projectile.DamageType = DamageClass.Default;
            Projectile.damage = 1;
            Projectile.knockBack = 0;

            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 150;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.ownerHitCheck = false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly || target.immortal || !target.active) return;
            if (Main.myPlayer == Projectile.owner)
            {
                SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
                if (adept.septima is AzureStriker && !(!target.active || target.life <= 0 || target.immortal ||
                    target.friendly))
                {
                    adept.TaggedNPCs.AddTag(target.whoAmI);

                    SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/DartHit") with
                    {
                        PitchVariance = 0.1f
                    }, Projectile.Center);
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/DartMiss") with
            {
                PitchVariance = 0.1f
            }, Projectile.Center);
            return base.OnTileCollide(oldVelocity);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetMaxDamage(1);
            base.ModifyHitNPC(target, ref modifiers);
        }
    }
}
