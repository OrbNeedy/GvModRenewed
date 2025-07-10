using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Common.GlobalNPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class VoltaicChainProjectile : ModProjectile
    {
        public static int BreakTime = 30;
        public static uint MoveTime = 120;

        private int ElectrocutionTime { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        private int WaitTime { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public bool Electrocuting { get; set; } = false;
        public List<int> PiercedEnemies { get; set; } = new();
        private Vector2 OriginPosition = Vector2.Zero;
        private int timer = 0;

        private Asset<Texture2D> ChainTip;
        private Asset<Texture2D> ChainSegments;
        private Asset<Texture2D> ElectrifyExtras;
        private Asset<Texture2D> ChainBreak;
        private int FrameTimer = 0;
        private int Frame = 0;
        private int ExtrasFrameTimer = 0;
        private int ExtrasFrame = 0;
        private bool InvisibleExtras = false;
        private bool FlippedExtras = false;
        private bool DarkExtras = false;

        public static SlotId soundID;
        public static ActiveSound soundInstance;

        public override void SetDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;
            Projectile.Size = new Vector2(50);
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 250;
            Projectile.knockBack = 3;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 0;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;
            Projectile.ownerHitCheck = false;
            Projectile.netImportant = true;
        }

        public override void SetStaticDefaults()
        {
            ChainTip = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/VoltaicChainProjectile");
            ChainSegments = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/ChainSegment");
            ElectrifyExtras = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/ChainExtras");
            ChainBreak = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/ChainBreak");
        }

        public override void OnSpawn(IEntitySource source)
        {
            ChainTip = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/VoltaicChainProjectile");
            ChainSegments = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/ChainSegment");
            ElectrifyExtras = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/ChainExtras");
            ChainBreak = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/ChainBreak");

            soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/ChainMove2") with
            {
                PitchVariance = 0.1f,
                Volume = 0.75f
            }, Projectile.Center);

            Projectile.timeLeft = ElectrocutionTime + WaitTime + BreakTime;
            OriginPosition = Projectile.Center;
        }

        public override void AI()
        {
            MoveTime = 60;
            if (timer == MoveTime)
            {
                Projectile.velocity.Normalize();
            }

            if (timer > WaitTime && Projectile.timeLeft > BreakTime)
            {
                Electrocuting = true;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
            } else
            {
                Electrocuting = false;
            }

            if (Electrocuting)
            {
                SoundEngine.TryGetActiveSound(soundID, out soundInstance);

                if (soundInstance == null)
                {
                    //Main.NewText("Playing sound");
                    soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/ChainElectrocuteConstant") with
                    {
                        PitchVariance = 0.1f,
                        Volume = 0.75f
                    }, Projectile.Center, StopSound);
                }
                else
                {
                    //Main.NewText("Sound state: " + soundInstance.IsPlaying);
                    if (!soundInstance.IsPlaying)
                    {
                        soundID = SoundEngine.
                            PlaySound(new SoundStyle("GvMod/Assets/Sfx/ChainElectrocuteConstant") with
                            {
                                PitchVariance = 0.1f,
                                Volume = 0.75f
                            }, Projectile.Center, StopSound);
                    }
                }
            }

            /*if (Projectile.timeLeft == BreakTime)
            {
                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/ChainBreak") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.75f
                }, Projectile.Center);
            }*/

            // Keep pierced enemies in place and release them one frame before Electrocuting is set to false
            if (Projectile.timeLeft > BreakTime - 1)
            {
                foreach (int index in PiercedEnemies)
                {
                    NPC target = Main.npc[index];
                    target.GetGlobalNPC<ChainedNPC>().Pierced = true;
                }
            } else
            {
                PiercedEnemies.Clear();
            }

            UpdateAnimation();

            timer++;
        }

        private bool StopSound(ActiveSound soundInstance)
        {
            if (Projectile.active && Projectile.timeLeft > BreakTime)
            {
                return true;
            }
            else
            {
                //Main.NewText("Stopping sound");
                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/ChainBreak") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.75f
                }, Projectile.Center);
                return false;
            }
        }

        private void UpdateAnimation()
        {
            if (Electrocuting)
            {
                if (FrameTimer >= 4)
                {
                    FrameTimer = 0;
                    Frame++;
                    if (Frame > 3)
                    {
                        Frame = 1;
                    }
                }

                if (ExtrasFrameTimer >= 3)
                {
                    ExtrasFrameTimer = 0;
                    ExtrasFrame++;
                    if (ExtrasFrame > 1)
                    {
                        ExtrasFrame = 0;
                    }
                    DarkExtras = !Main.rand.NextBool(7);
                    FlippedExtras = !Main.rand.NextBool(3);
                    InvisibleExtras = !Main.rand.NextBool(5);
                }

                ExtrasFrameTimer++;

                
            } else if (Projectile.timeLeft <= BreakTime)
            {
                if (Projectile.timeLeft == BreakTime)
                {
                    Frame = 0;
                }
                if (FrameTimer >= 7 && Frame < 4)
                {
                    FrameTimer = 0;
                    Frame++;
                }
            }
            FrameTimer++;
        }

        public override bool ShouldUpdatePosition()
        {
            return timer < MoveTime;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (PiercedEnemies.Contains(target.whoAmI))
            {
                return false;
            }

            return base.CanHitNPC(target);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!Electrocuting)
            {
                modifiers.SetMaxDamage(10);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.velocity.Length() > 10 && !PiercedEnemies.Contains(target.whoAmI) && !Electrocuting &&
                !target.friendly && !target.immortal)
            {
                PiercedEnemies.Add(target.whoAmI);
                target.oldPosition = target.position;
                target.GetGlobalNPC<ChainedNPC>().Pierced = true;
                target.netUpdate = true;
            }
            // Add chained effect (Enemies get stopped while pierced)
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Frame < 3)
            {
                Main.EntitySpriteDraw(
                    ChainTip.Value,
                    Projectile.Center - Main.screenPosition,
                    new Rectangle(0, 62 * Frame, 80, 62),
                    Color.White,
                    Projectile.velocity.ToRotation(),
                    new Vector2(4, 31),
                    Projectile.scale,
                    SpriteEffects.None
                );
            }

            return false;
        }

        public override bool PreDrawExtras()
        {
            if (Projectile.timeLeft > BreakTime)
            {
                float segments = Projectile.Center.Distance(OriginPosition) / 364f;

                Vector2 separation = Projectile.Center.DirectionTo(OriginPosition) * 364f;

                for (int i = 0; i <= segments; i++)
                {
                    Vector2 currentPosition = Projectile.Center.DirectionTo(OriginPosition) * (364 * i);

                    if (Frame < 3)
                    {
                        Rectangle bounds = new Rectangle(0, 46 * Frame, 368, 46);
                        float distanceToTarget = currentPosition.Distance(OriginPosition);

                        if (distanceToTarget <= 364)
                        {
                            bounds.Width = (int)(distanceToTarget / 364f);
                        }
                        ChainTip = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/VoltaicChainProjectile");
                        ChainSegments = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/ChainSegment");
                        ElectrifyExtras = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/ChainExtras");
                        ChainBreak = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/ChainBreak");
                        Main.EntitySpriteDraw(
                            ChainSegments.Value,
                            Projectile.Center - Main.screenPosition + currentPosition,
                            bounds,
                            Color.White,
                            Projectile.velocity.ToRotation(),
                            new Vector2(368, 23),
                            Projectile.scale,
                            SpriteEffects.None
                        );
                    }

                    if (Electrocuting && !InvisibleExtras)
                    {
                        Main.EntitySpriteDraw(
                            ElectrifyExtras.Value,
                            Projectile.Center - Main.screenPosition + currentPosition,
                            new Rectangle(0, 130 * ExtrasFrame, 338, 130),
                            DarkExtras ? new Color(0.5f, 0.5f, 0.5f) : Color.White,
                            Projectile.velocity.ToRotation(),
                            new Vector2(338, 65),
                            Projectile.scale,
                            FlippedExtras ? SpriteEffects.FlipVertically : SpriteEffects.None
                        );
                    }
                }
            } else
            {
                if (Frame >= 4) return base.PreDrawExtras();
                float segments = Projectile.Center.Distance(OriginPosition) / 104f;

                Vector2 separation = Projectile.Center.DirectionTo(OriginPosition) * 104f;
                for (int i = 0; i <= segments; i++)
                {
                    Vector2 currentPosition = Projectile.Center.DirectionTo(OriginPosition) * (104 * i);

                    Rectangle bounds = new Rectangle(0, 156 * Frame, 208, 156);
                    float distanceToTarget = currentPosition.Distance(OriginPosition);

                    if (distanceToTarget <= 208)
                    {
                        bounds.Width = (int)(distanceToTarget / 208f);
                    }

                    Main.EntitySpriteDraw(
                        ChainBreak.Value,
                        Projectile.Center - Main.screenPosition + currentPosition,
                        bounds,
                        Color.White,
                        Projectile.velocity.ToRotation(),
                        new Vector2(208, 78),
                        Projectile.scale,
                        SpriteEffects.None
                    );
                }
            }
            return base.PreDrawExtras();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Electrocuting)
            {
                // Box to limit the line collision
                // Vs the destroyer it causes lag issues due to the amount of calculations
                float lowerX = OriginPosition.X < Projectile.Center.X ? OriginPosition.X : Projectile.Center.X;
                float greaterX = OriginPosition.X > Projectile.Center.X ? OriginPosition.X : Projectile.Center.X;
                float lowerY = OriginPosition.Y < Projectile.Center.Y ? OriginPosition.Y : Projectile.Center.Y;
                float greaterY = OriginPosition.Y > Projectile.Center.Y ? OriginPosition.Y : Projectile.Center.Y;

                Vector2 BoxPosition = new Vector2(lowerX, lowerY);
                Vector2 BoxSize = new Vector2(greaterX - lowerX, greaterY - lowerY);

                if (!Collision.CheckAABBvAABBCollision(BoxPosition, BoxSize, targetHitbox.TopLeft(), 
                    targetHitbox.Size()))
                {
                    return false;
                }

                float pointRef = 0;
                /*Main.NewText("Electrocution collision: " + (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), 
                    targetHitbox.Size(), OriginPosition, Projectile.Center, 26, ref pointRef)));*/
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), 
                    OriginPosition, Projectile.Center, 26, ref pointRef);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
    }
}
