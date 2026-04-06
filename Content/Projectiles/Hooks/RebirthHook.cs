using GvMod.Common.Players.Sevenths;
using GvMod.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;

namespace GvMod.Content.Projectiles.Hooks
{
    class RebirthHook : ModProjectile
    {
        public static Asset<Texture2D> chainTexture;

        public override void Load()
        { 
            chainTexture = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/Hooks/RebirthHookChain");
        }

        public override void Unload()
        {
            chainTexture = null;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SingleGrappleHook[Type] = false;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 6;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/RopeHookThrow") with
            {
                Volume = 0.5f,
                PitchVariance = 0.1f
            }, Projectile.Center);

            /*if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<RebirthHitbox>(), Projectile.damage,
                    Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
            }*/
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < Rebirth.MinGrappleSpeed && 
                Projectile.oldVelocity.Length() >= Rebirth.MinGrappleSpeed)
            {
                //Main.NewText("Play sound here");
                /*
                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/RopeHookContract") with
                {
                    Volume = 0.5f,
                    PitchVariance = 0.1f
                }, Projectile.Center);*/
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        // Amethyst Hook is 300, Static Hook is 600.
        public override float GrappleRange()
        {
            return 1000f;
        }

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 1; // The amount of hooks that can be shot out
        }

        public override bool? CanUseGrapple(Player player)
        {
            int hooksOut = 0;
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.owner == Main.myPlayer && projectile.type == Projectile.type)
                {
                    hooksOut++;
                }
            }

            return hooksOut <= 1;
        }

        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 40f / (float)Projectile.extraUpdates;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = Rebirth.MinGrappleSpeed + 2f;
        }

        // Can customize what tiles this hook can latch onto, or force/prevent latching altogether, like Squirrel Hook also latching to trees
        public override bool? GrappleCanLatchOnTo(Player player, int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (TileID.Sets.IsATreeTrunk[tile.TileType] || tile.TileType == TileID.PalmTree || 
                tile.TileType == TileID.Rope || tile.TileType == TileID.MysticSnakeRope ||
                tile.TileType == TileID.SilkRope || tile.TileType == TileID.VineRope ||
                tile.TileType == TileID.WebRope)
            {
                return true;
            }

            return null;
        }

        // Not using Vector2's DirectionTo and DistanceTo is insane 
        public override bool PreDrawExtras()
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 directionToPlayer = playerCenter - Projectile.Center;
            float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
            float distanceToPlayer = directionToPlayer.Length();

            while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer))
            {
                directionToPlayer /= distanceToPlayer; // get unit vector
                directionToPlayer *= chainTexture.Height() - 4; // multiply by chain link length

                center += directionToPlayer; // update draw position
                directionToPlayer = playerCenter - center; // update distance
                distanceToPlayer = directionToPlayer.Length();

                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                // Draw chain
                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
                    chainTexture.Value.Bounds, drawColor, chainRotation,
                    chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            // Stop vanilla from drawing the default chain.
            return false;
        }
    }
}
