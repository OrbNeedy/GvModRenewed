using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public enum OrbitsBehavior
    {
        Default, 
        Launch,
        Spread, 
        Electroshock, 
        ElectroshockCounterclock
    }

    public class AstrasphereOrbits : ModProjectile
    {
        private int fieldIndex { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        private float baseRotation { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        private int behavior { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        private Vector2 target = Vector2.Zero;
        private int distance = 122;

        private int frame = 0;
        private int frameTimer = 0;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(60);
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 33;
            Projectile.knockBack = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 15;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 100;
            Projectile.ownerHitCheck = false;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            target = Projectile.Center;
            Projectile.Center = Projectile.Center + new Vector2(0, 122).RotatedBy(baseRotation);
            switch (behavior)
            {
                case (int)OrbitsBehavior.Launch:
                    Projectile.timeLeft += 100;
                    break;
                case (int)OrbitsBehavior.Spread:
                    Projectile.timeLeft += 200;
                    break;
                case (int)OrbitsBehavior.ElectroshockCounterclock:
                case (int)OrbitsBehavior.Electroshock:
                    distance = 0;
                    Projectile.Center = target;
                    Projectile.timeLeft += 400;
                    break;
            }

            Projectile.netUpdate = true;
            base.OnSpawn(source);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(target);
            writer.Write7BitEncodedInt(distance);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            target = reader.ReadVector2();
            distance = reader.Read7BitEncodedInt();
            base.ReceiveExtraAI(reader);
        }

        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                if (fieldIndex >= 0)
                {   
                    Projectile targetField = Main.projectile[fieldIndex];
                    if (targetField.active && targetField.owner == Projectile.owner &&
                        (targetField.ModProjectile is AstrasphereProjectile ||
                        targetField.ModProjectile is FlashphereProjectile))
                    {
                        target = targetField.Center;
                    }
                    else if (behavior == (int)OrbitsBehavior.Spread)
                    {
                        distance += 6;
                    }
                }
                if (behavior == (int)OrbitsBehavior.Electroshock || behavior == (int)OrbitsBehavior.ElectroshockCounterclock)
                {
                    distance += 4;
                }
                Projectile.Center = target + new Vector2(0, distance).RotatedBy(baseRotation);
                Projectile.netUpdate = true;
            }

            TextureCycles();
            if (behavior == (int)OrbitsBehavior.ElectroshockCounterclock)
            {
                baseRotation -= MathHelper.TwoPi / 100;
            } else
            {
                baseRotation += MathHelper.TwoPi / 100;
            }
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
