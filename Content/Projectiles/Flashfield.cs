using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public enum FlashfieldBehavior
    {
        Default, // Follow the owner
        Launch // Gets sent to the mouse position
    }
    public class Flashfield : ModProjectile
    {
        private int Behavior { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        private Vector2 target = new Vector2(0, 0);
        private int timer = 0;
        private int cycle = 0;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(242);
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = DamageClass.Generic; //ModContent.GetInstance<SeptimaDamageClass>();
            Projectile.damage = 1;
            Projectile.knockBack = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
            Projectile.ownerHitCheck = false;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Play sound effect
            // TODO: Separate the sound effect into a start and begining
        }

        public override void AI()
        {
            switch (Behavior)
            {
                case (int)FlashfieldBehavior.Launch:
                    if (Main.myPlayer == Projectile.owner)
                    {
                        target = Main.MouseWorld;
                    }
                    break;
                default:
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.Center = Main.LocalPlayer.Center;
                        Projectile.netUpdate = true;
                    }
                    break;
            }
            timer++;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 ellipsePosition = new Vector2(projHitbox.Left, projHitbox.Top);
            Vector2 ellipseDimentions = new Vector2(projHitbox.Width, projHitbox.Height);
            Vector2 ellipseCenter = ellipsePosition + 0.5f * ellipseDimentions;
            ellipseDimentions *= Projectile.scale;
            float x = 0f;
            float y = 0f;
            if (targetHitbox.Left > ellipseCenter.X)
            {
                x = targetHitbox.Left - ellipseCenter.X;
            }
            else if (targetHitbox.Left + targetHitbox.Width < ellipseCenter.X)
            {
                x = targetHitbox.Left + targetHitbox.Width - ellipseCenter.X;
            }
            if (targetHitbox.Top > ellipseCenter.Y)
            {
                y = targetHitbox.Top - ellipseCenter.Y;
            }
            else if (targetHitbox.Top + targetHitbox.Height < ellipseCenter.Y)
            {
                y = targetHitbox.Top + targetHitbox.Height - ellipseCenter.Y;
            }
            float a = ellipseDimentions.X / 2f;
            float b = ellipseDimentions.Y / 2f;
            return (x * x) / (a * a) + (y * y) / (b * b) <= 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D field = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/Flashfield").Value;
            Main.EntitySpriteDraw(
                field, 
                Projectile.Center - Main.screenPosition, 
                field.Bounds, 
                Color.White, 
                0, 
                field.Bounds.Size() * 0.5f, 
                1f, SpriteEffects.None
            );

            return false;
        }
    }
}
