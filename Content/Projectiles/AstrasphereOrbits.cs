using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class AstrasphereOrbits : ModProjectile
    {
        private int fieldIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        private float baseRotation { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }

        private int frame = 0;
        private int frameTimer = 0;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(60);
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = ModContent.GetInstance<SeptimaDamage>();
            Projectile.damage = 33;
            Projectile.knockBack = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.ownerHitCheck = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Center = Projectile.Center + new Vector2(0, 122).RotatedBy(baseRotation);
            base.OnSpawn(source);
        }

        public override void AI()
        {
            Vector2 target = Vector2.Zero;

            if (Main.myPlayer == Projectile.owner)
            {
                if (fieldIndex > 0)
                {
                    Projectile targetField = Main.projectile[fieldIndex];
                    if (targetField.active && targetField.owner == Projectile.owner && 
                        targetField.ModProjectile is Flashfield && 
                        (targetField.ai[0] == (int)FlashfieldBehavior.Astrasphere || 
                        targetField.ai[0] == (int)FlashfieldBehavior.Launch))
                    {
                        target = targetField.Center;
                    }
                }
                target = Main.projectile[fieldIndex].Center;
                Projectile.Center = target + new Vector2(0, 122).RotatedBy(baseRotation);
            }

            TextureCycles();
            baseRotation += MathHelper.TwoPi / 100;
            Projectile.netUpdate = true;
            base.AI();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void TextureCycles()
        {
            if (Projectile.timeLeft <= 4)
            {
                frame = 3;
                return;
            }
            if (frameTimer >= 4)
            {
                frame++;
                frameTimer = 0;
                if (frame > 2)
                {
                    frame = 1;
                }
            }
            // Add ending frames
            frameTimer++;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/AstrasphereOrbits");
            Main.EntitySpriteDraw(
                texture.Value,
                Projectile.Center - Main.screenPosition,
                new Rectangle(84 * frame, 0, 84, 84),
                Color.White,
                0,
                new Vector2(42, 42),
                1f, SpriteEffects.None
            );
            return false;
        }
    }
}
