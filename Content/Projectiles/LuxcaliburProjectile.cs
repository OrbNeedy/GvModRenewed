using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public enum LuxcaliburBehavior
    {
        Default, 
        Launch, 
        LaunchAnthem, 
        SlashWave
    }
    public class LuxcaliburProjectile : ModProjectile
    {
        private int Behavior { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        private int timer = 0;
        private int cycle = 0;
        private Asset<Texture2D> field;
        private List<float[]> oldPositions = new List<float[]>();
        private Asset<Texture2D> extras;
        private float extrasRotation = 0;

        private Rectangle bounds = new Rectangle(0, 0, 192, 108);
        private Vector2 visualScale = new Vector2(0.125f, 0.4f);
        private bool darken = false;
        private int extrasFrame = 0;
        private int frame = 0;
        private int frameTimer = 0;
        private bool hideExtras = false;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(200);
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 250;
            Projectile.knockBack = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 30;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;
            Projectile.ownerHitCheck = false;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            //Main.NewText("Speed: " + Projectile.velocity.Length());
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/LuxcaliburUse") with
            {
                PitchVariance = 0.1f,
                Volume = 0.75f
            }, Projectile.Center);

            switch (Behavior)
            {
                case (int)LuxcaliburBehavior.Launch:
                    field = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/LuxcaliburProjectile");
                    Projectile.timeLeft += 120;
                    Projectile.netUpdate = true;
                    break;
                default:
                    field = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/LuxcaliburProjectile");
                    Projectile.netUpdate = true;
                    break;
            }
            extras = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/AstrasphereExtras");
            timer++;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(cycle);
            writer.Write7BitEncodedInt(timer);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            cycle = reader.Read7BitEncodedInt();
            timer = reader.Read7BitEncodedInt();
            base.ReceiveExtraAI(reader);
        }

        public override void AI()
        {
            //Main.NewText("Speed: " + Projectile.velocity.Length());
            switch (Behavior)
            {
                case (int)LuxcaliburBehavior.Launch:
                    if (cycle == 0)
                    {
                        if (Projectile.velocity.Length() > 0.001f)
                        {
                            Projectile.velocity *= 0.8f;
                        }
                        if (timer >= 75) 
                        {
                            timer = 0;
                            cycle++;
                            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 6;
                        }
                    } else
                    {
                        if (Projectile.velocity.Length() < 16f)
                        {
                            Projectile.velocity += Vector2.Normalize(Projectile.velocity) * 0.25f;
                        }
                        if (timer%6 == 0 || timer == 0)
                        {
                            oldPositions.Add([Projectile.Center.X, Projectile.Center.Y, 1, 
                                Projectile.rotation]);
                        }
                    }
                    break;
                default:
                    //Main.NewText("Default Luxcalibur behavior");
                    if (Projectile.velocity.Length() > 0.001f)
                    {
                        Projectile.velocity *= 0.8f;
                    }
                    break;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            timer++;
            TextureCycles();
        }

        public override bool ShouldUpdatePosition()
        {
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/LuxcaliburEnd") with
            {
                PitchVariance = 0.1f,
                Volume = 0.75f
            }, Projectile.Center);
            base.OnKill(timeLeft);
        }

        private void TextureCycles()
        {
            for (int i = 0; i < oldPositions.Count; i++)
            {
                oldPositions[i][2] -= 0.06f;
            }
            oldPositions.RemoveAll((x) => { return x[2] <= 0; });
            switch (Behavior)
            {
                default:
                    if (frame < 2)
                    {
                        if (frameTimer >= 6)
                        {
                            frame++;
                            if (Projectile.velocity.Length() > 6 && frame >= 2)
                            {
                                frame = 1;
                            }
                            frameTimer = 0;
                        }

                        RecalculateScale();
                    }

                    if (Projectile.timeLeft <= 12)
                    {
                        frame = 3;
                    }
                    
                    if (Projectile.timeLeft <= 6)
                    {
                        darken = true;
                        if (visualScale.Y > 0)
                        {
                            visualScale.Y -= 0.3333f;
                        }
                    }
                    break;
            }

            bounds.Y = frame * bounds.Height;

            frameTimer++;
        }

        private void RecalculateScale()
        {
            if (visualScale.X < 1)
            {
                visualScale.X += 0.219f;
            }
            else
            {
                visualScale.X = 1;
            }

            if (visualScale.Y < 1)
            {
                visualScale.Y += 0.15f;
            }
            else
            {
                visualScale.Y = 1;
            }
        }
        public override bool? CanCutTiles()
        {
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0;
            Vector2 tip = (Projectile.Center + new Vector2(86, 0)).RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 centerBase = (Projectile.Center + new Vector2(-86, 0)).
                RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 base1 = (Projectile.Center + new Vector2(-86, -54)).
                RotatedBy(Projectile.rotation, Projectile.Center);
            Vector2 base2 = (Projectile.Center + new Vector2(-86, 54)).
                RotatedBy(Projectile.rotation, Projectile.Center);
            Rectangle boxBounds = new Rectangle((int)(Projectile.Center.X - 200),
                (int)(Projectile.Center.Y - 200), 400, 400);

            // First check the AABB before checking line collision to save time on calculating it
            if (!boxBounds.Intersects(targetHitbox))
            {
                return false;
            }

            bool line1 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), tip, base1, 
                4, ref collisionPoint);
            bool line2 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), base1, base2,
                4, ref collisionPoint);
            bool line3 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), tip, base2,
                4, ref collisionPoint); 
            bool line4 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), tip, 
                centerBase, 4, ref collisionPoint);

            return line1 || line2 || line3 || line4;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            foreach (var afterimage in oldPositions)
            {
                Rectangle afterimageBounds = new Rectangle(0, 108 * 2, 192, 108);
                Main.EntitySpriteDraw(
                    field.Value,
                    new Vector2(afterimage[0], afterimage[1]) - Main.screenPosition,
                    afterimageBounds, 
                    Color.White * afterimage[2],
                    afterimage[3],
                    afterimageBounds.Size() * 0.5f,
                    1, 
                    SpriteEffects.None
                );
            }

            Main.EntitySpriteDraw(
                field.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                darken ? new Color(0.5f, 0.5f, 0.5f) * 0.8f : Color.White,
                Projectile.rotation,
                bounds.Size() * 0.5f,
                visualScale, 
                SpriteEffects.None
            );

            return false;
        }
    }
}
