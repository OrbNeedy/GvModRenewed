using GvMod.Common;
using GvMod.Common.Players;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class QueenSlime : ModProjectile
    {
        public Vector2 targetPosition = Vector2.Zero;
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int State { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public bool Flying { get => Projectile.ai[2] == 1; set => Projectile.ai[2] = value ? 1 : 0; }
        public int Timer { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public int StateCounter { get => (int)Projectile.localAI[1]; set => Projectile.localAI[1] = value; }
        public int JumpCounter { get => (int)Projectile.localAI[2]; set => Projectile.localAI[2] = value; }
        private int minFrame = 0;
        private int maxFrame = 4;
        private int wingFrame = 0;
        private int wingFrameCounter = 0;
        public SpriteBatchState prevState = null;

        public override void SetDefaults()
        {
            Projectile.width = (int)(TextureAssets.Npc[NPCID.QueenSlimeBoss].Size().X / 2f);
            Projectile.height = (int)(TextureAssets.Npc[NPCID.QueenSlimeBoss].Size().Y / 16f);
            Main.projFrames[Projectile.type] = 16;
            Projectile.frame = 0;
            Projectile.light = 0.75f;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 3600; // 3600
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/NPC_{NPCID.QueenSlimeBoss}";

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.netUpdate = true;
            //Main.NewText("State: " + State);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
        }

        public override void AI()
        {
            TargetingSystem();
            Vector2 newVelocity = Projectile.velocity;
            float lerpAmount = 0.08f; // Does nothing if not flying
            bool despawnAllow = false;
            bool allowGroundCollision = true;
            float halfWidth = Projectile.width / 2f;
            float halfHeight = Projectile.height / 2f;

            Rectangle down = new Rectangle(
                    (int)(Projectile.Center.X - halfWidth), (int)(Projectile.Center.Y + (halfHeight * 0.6f)),
                    Projectile.width, (int)(halfHeight * 0.4f)
                );

            bool groundCollision = Collision.SolidCollision(down.TopLeft(), down.Width, down.Height);

            if (Flying)
            {
                minFrame = 4;
                maxFrame = 8;
            }
            else
            {
                minFrame = 0;
                maxFrame = 4;
            }

            if (State != -1)
            {
                if (TargetNPC != -1)
                {
                    switch (State)
                    {
                        case 0:
                            // Follow and stomp
                            Phase1Behavior(ref newVelocity, ref lerpAmount, ref despawnAllow, groundCollision);
                            break;
                        case 1:
                            // Fly and stomp
                            Phase2Behavior(ref newVelocity, ref lerpAmount, ref despawnAllow, groundCollision);
                            break;
                    }
                }
                else
                {
                    // Follow player
                    IdleBehavior(ref newVelocity, ref lerpAmount, ref despawnAllow, groundCollision);
                }

                if (despawnAllow && Projectile.timeLeft <= 3)
                {
                    State = -1;
                    Timer = 0;
                    JumpCounter = 0;
                }
            } else
            {
                DespawnBehavior(ref newVelocity, ref lerpAmount, ref despawnAllow, ref allowGroundCollision, groundCollision);

                if (Timer > 600 || Projectile.Center.X <= 0 || Projectile.Center.X >= Main.maxTilesY)
                {
                    Projectile.timeLeft = -1;
                    Projectile.active = false;
                    return;
                } else
                {
                    Projectile.timeLeft++;
                }
            }

            if (Flying)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, newVelocity, lerpAmount);
                Projectile.rotation = Projectile.velocity.X * 0.06f;
            } else
            {
                if (allowGroundCollision)
                {
                    StopAtBlocks(ref newVelocity, ref lerpAmount, groundCollision);
                }

                if (!groundCollision) newVelocity.Y += 0.2f;
                Projectile.velocity = newVelocity;
            }

            ControlAnimations();
            Timer++;
        }

        public void DespawnBehavior(ref Vector2 newVelocity, ref float lerpAmount, ref bool allowDespawn,
            ref bool allowGroundCollision, bool groundCollision)
        {
            if (Flying)
            {
                lerpAmount = 0.008f;
                newVelocity = new Vector2(0, -12);
            } else
            {
                if (JumpCounter < 3)
                {
                    if (groundCollision)
                    {
                        Projectile.frame = 4;
                        int direction = 1;
                        if (StateCounter < 0) direction = -1;
                        newVelocity = new Vector2(6 * direction, -6 - (JumpCounter * 4));
                        JumpCounter++;
                    }
                } else
                {
                    allowGroundCollision = false;
                }
            }
        }

        public void Phase1Behavior(ref Vector2 newVelocity, ref float lerpAmount, ref bool allowDespawn, 
            bool groundCollision)
        {
            float distance = Projectile.Center.Distance(targetPosition);
            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);

            if (Projectile.velocity.Y < -1)
            {
                minFrame = 4;
                if (Projectile.frame >= 6)
                {
                    minFrame = 6;
                }
                maxFrame = 7;
            }

            if (Projectile.velocity.Y > 1)
            {
                minFrame = 8;
                maxFrame = 9;
            }

            if (Timer >= 120)
            {
                if (JumpCounter < 3)
                {
                    if (groundCollision)
                    {
                        Projectile.frame = 4;
                        newVelocity = new Vector2(direction.X * 6, -8);
                        JumpCounter++;
                        Timer = 0;
                    }
                } else
                {
                    if (StateCounter % 2 == 0)
                    {
                        // Charge Dust
                        SpawnChargeDust();

                        minFrame = 13;
                        maxFrame = 16;

                        if (Timer >= 150)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int projNum = 7;
                                float angle = MathHelper.Pi / (float)projNum;
                                for (int i = 1; i <= projNum; i++)
                                {
                                    Vector2 shootDirection = new Vector2(-8, 0).RotatedBy(angle * i);
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                        shootDirection, ModContent.ProjectileType<RoyalGel>(), Projectile.damage,
                                        1.75f, Projectile.owner);
                                }
                            }
                            Timer = 0;
                            StateCounter++;
                            JumpCounter = 0;
                        }
                    } else
                    {
                        if (groundCollision)
                        {
                            // Jump and fall
                            if (Timer == 120)
                            {
                                Projectile.frame = 4;
                                newVelocity = new Vector2(direction.X * 6, -14);
                            }
                            if (Timer > 120)
                            {
                                // Collision dust
                                SpawnCollisionDust();
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                        Vector2.Zero, ModContent.ProjectileType<QueenSmash>(),
                                        (int)(Projectile.damage * 1.5f), 4, Projectile.owner);
                                }
                                Timer = 0;
                                StateCounter++;
                                JumpCounter = 0;
                            }
                        } else
                        {
                            if (Projectile.velocity.Y > 0)
                            {
                                minFrame = 12;
                                maxFrame = 13;
                                newVelocity = new Vector2(0, 18);

                                // Slam dust
                                SpawnSlamDust();
                            }
                        }
                    }
                }
            } else if (groundCollision && Projectile.timeLeft <= 1800)
            {
                if (!Flying)
                {
                    Flying = true;
                    Projectile.netUpdate = true;
                }

                State = 1;
            }
        }

        public void Phase2Behavior(ref Vector2 newVelocity, ref float lerpAmount, ref bool allowDespawn,
            bool groundCollision)
        {
            float distance = Projectile.Center.Distance(targetPosition);
            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);

            lerpAmount = 0.06f;
            newVelocity = Projectile.Center.DirectionTo(targetPosition + 
                new Vector2(160 * MathF.Sin(Timer * 0.1f), -320)) * 10;

            if (StateCounter % 2 == 0)
            {
                if (Timer >= 180 && (direction.X * distance) <= 192)
                {
                    lerpAmount = 1;
                    newVelocity = new Vector2(0, 18);

                    // Slam dust
                    SpawnSlamDust();

                    float bottomLevel = Projectile.Center.Y + (Projectile.height / 2f);
                    maxFrame = minFrame + 1; 
                    wingFrame = 0;
                    wingFrameCounter = 0;

                    if (Timer >= 240)
                    {
                        Timer = 0;
                        StateCounter++;
                    }

                    if (groundCollision && bottomLevel >= targetPosition.Y)
                    {
                        // Collision dust
                        SpawnCollisionDust();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                Vector2.Zero, ModContent.ProjectileType<QueenSmash>(),
                                (int)(Projectile.damage * 1.5f), 4, Projectile.owner);
                        }
                        Timer = 0;
                        StateCounter++;
                        newVelocity.Y = 0;
                    }
                }
            } else
            {
                if (Timer >= 90)
                {
                    // Charge Dust
                    SpawnChargeDust();
                    if (Timer >= 180)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int projNum = 10;
                            float angle = MathHelper.TwoPi / (float)projNum;
                            Vector2 baseDirection = direction * 8;
                            for (int i = 0; i < projNum; i++)
                            {
                                Vector2 shootDirection = baseDirection.RotatedBy(angle * i);
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                    shootDirection, ModContent.ProjectileType<RoyalGel>(), Projectile.damage,
                                    1.75f, Projectile.owner);
                            }
                        }
                        Timer = 0;
                        StateCounter++;
                    }
                }
            }
        }

        public void IdleBehavior(ref Vector2 newVelocity, ref float lerpAmount, ref bool allowDespawn, bool groundCollision)
        {
            float distance = Projectile.Center.Distance(targetPosition);
            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);

            if (!Flying)
            {
                float bottomLevel = Projectile.Center.Y + (Projectile.height / 2f);

                if (Timer >= 120 && groundCollision && MathF.Abs(distance * direction.X) > 352 && 
                    bottomLevel >= targetPosition.Y)
                {
                    newVelocity = new Vector2(direction.X * 6, -8);
                    Timer = 0;
                }
            } else
            {
                if (Projectile.velocity.Y < -1)
                {
                    minFrame = 4;
                    if (Projectile.frame >= 6)
                    {
                        minFrame = 6;
                    }
                    maxFrame = 7;
                }

                if (Projectile.velocity.Y > 1)
                {
                    minFrame = 8;
                    maxFrame = 9;
                }

                Vector2 offset = new Vector2(-256, 448);
                Vector2 ownerPosition = targetPosition;

                if (TargetNPC != -1)
                {
                    offset.X *= Main.npc[TargetNPC].direction;
                } else
                {
                    offset.X *= Main.player[Projectile.owner].direction;
                    ownerPosition = Main.player[Projectile.owner].Center;
                }

                newVelocity = Projectile.DirectionTo(ownerPosition + offset);
            }
        }

        public void StopAtBlocks(ref Vector2 newVelocity, ref float lerpAmount, bool groundCollision)
        {
            float bottomLevel = Projectile.Center.Y + (Projectile.height / 2f);

            if (groundCollision && bottomLevel >= targetPosition.Y)
            {
                // Slow down when on solid ground
                newVelocity.X *= 0.85f;
                // If going down, stop
                if (Projectile.velocity.Y > 0)
                {
                    newVelocity.Y = 0;
                    Projectile.position.Y = bottomLevel - Projectile.height + 0.001f;
                }
            }
        }

        public void SpawnChargeDust()
        {
            Color dustColor = NPC.AI_121_QueenSlime_GetDustColor();
            for (int i = 0; i < 6; i++)
            {
                Vector2 vel = new Vector2(
                    Main._rand.NextFloat(0, 2), 0).RotatedByRandom(MathHelper.TwoPi);
                int num28 = Dust.NewDust(Projectile.Center,
                    0, 0, DustID.TintableDust, vel.X,
                    vel.Y, 50, dustColor, 1.5f);
                Main.dust[num28].noGravity = true;
                Main.dust[num28].noLightEmittence = false;
                Main.dust[num28].velocity = Main.dust[num28].position.DirectionTo(Projectile.Center) * 8;
            }
        }

        public void SpawnCollisionDust()
        {
            Color dustColor = NPC.AI_121_QueenSlime_GetDustColor();
            for (int i = 0; i < 12; i++)
            {
                int num28 = Dust.NewDust(Projectile.position + Vector2.UnitX * -20f,
                    Projectile.width + 40, Projectile.height, DustID.TintableDust,
                    Projectile.velocity.X, Projectile.velocity.Y, 50, dustColor, 1.5f);
                Main.dust[num28].noGravity = true;
                Main.dust[num28].noLightEmittence = false;
            }
        }

        public void SpawnSlamDust()
        {
            Color dustColor = NPC.AI_121_QueenSlime_GetDustColor();
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = new Vector2(
                    Main._rand.NextFloat(-1, 1) * Projectile.width / 2f,
                    Projectile.height / 2f);
                float velX = Main._rand.NextFloat(1, 10) * offset.X < 0 ? -1 : 1;
                int num28 = Dust.NewDust(Projectile.Center + offset,
                    0, 0, DustID.TintableDust, velX,
                    Projectile.velocity.Y, 50, dustColor, 1.5f);
                Main.dust[num28].noGravity = true;
                Main.dust[num28].noLightEmittence = false;
            }
        }

        public void TargetingSystem()
        {
            float maxDistance = 2400;

            if (TargetNPC == -1)
            {
                NPCTags ownerTags = Main.player[Projectile.owner].GetModPlayer<SeptimaPlayer>().TaggedNPCs;
                foreach (NPC target in Main.ActiveNPCs)
                {
                    if (ValidTarget(target) && (target.Distance(Projectile.Center) <= maxDistance
                        || ownerTags.GetTag(target.whoAmI).targetIndex != -1))
                    {
                        TargetNPC = target.whoAmI;
                        targetPosition = target.Center;
                        break;
                    }
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    targetPosition = Main.MouseWorld;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                NPCTags ownerTags = Main.player[Projectile.owner].GetModPlayer<SeptimaPlayer>().TaggedNPCs;

                if (!ValidTarget(Main.npc[TargetNPC]) || (ownerTags.GetTag(TargetNPC).targetIndex == -1 &&
                    Main.npc[TargetNPC].Distance(Projectile.Center) > maxDistance))
                {
                    TargetNPC = -1;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        targetPosition = Main.MouseWorld;
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    targetPosition = Main.npc[TargetNPC].Center;
                }
            }
            //Main.NewText("Target position after targeting: " + targetPosition);
            //Main.NewText("Projectile position: " + Projectile.Center + "\n\n");
        }

        public bool ValidTarget(NPC target)
        {
            if (!target.friendly && target.life > 0 && target.type != NPCID.TargetDummy &&
                target.CanBeChasedBy() && target.active)
            {
                return true;
            }
            return false;
        }

        public void ControlAnimations()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }

            if (Projectile.frame >= maxFrame)
            {
                Projectile.frame = minFrame;
            }

            if (Projectile.frame < minFrame)
            {
                Projectile.frame = minFrame;
            }

            wingFrameCounter++;
            if (wingFrameCounter >= 6)
            {
                wingFrame++;
                wingFrameCounter = 0;
                if (wingFrame >= 4)
                {
                    wingFrame = 0;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft -= 2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 4;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
            Vector2 additionalVel = Projectile.velocity * 0.9f;

            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 1258);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 1259);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 1259);
            /*Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 383);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 384);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 385);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 386);*/
        }

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> crystal = TextureAssets.Extra[ExtrasID.QueenSlimeCrystalCore];
            if (Flying)
            {
                Asset<Texture2D> wings = TextureAssets.Extra[ExtrasID.QueenSlimeWing];
                Rectangle wingBounds = wings.Frame(1, 4, 0, wingFrame);

                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 wingOffset = new Vector2(-84 * i, -26).RotatedBy(Projectile.rotation);
                    Main.EntitySpriteDraw(
                        wings.Value,
                        Projectile.Center - wingOffset - Main.screenPosition,
                        wingBounds,
                        Color.White,
                        Projectile.rotation,
                        wingBounds.Size() / 2f,
                        1,
                        i < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally
                    );
                }
            }

            Main.EntitySpriteDraw(
                crystal.Value,
                Projectile.Center - Main.screenPosition,
                crystal.Frame(),
                Color.White,
                Projectile.rotation,
                crystal.Size() / 2f,
                1,
                SpriteEffects.None
            );
            return base.PreDrawExtras();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> asset = TextureAssets.Npc[NPCID.QueenSlimeBoss];
            int xFrame = 0;

            if (Flying)
            {
                xFrame = 1;
            }
            Rectangle bounds = asset.Frame(2, Main.projFrames[Projectile.type], xFrame, Projectile.frame);
            Asset<Texture2D> crown = TextureAssets.Extra[ExtrasID.QueenSlimeCrown];

            // Add shader
            prevState = SpriteBatchExt.GetState(Main.spriteBatch);
            SpriteBatchExt.Restart(Main.spriteBatch, prevState, SpriteSortMode.Immediate);
            GameShaders.Misc["QueenSlime"].Apply();
            //int oldShader = Main.CurrentDrawnEntityShader;
            DrawData data = new DrawData(
                asset.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                Color.White,
                Projectile.rotation,
                bounds.Size() / 2f,
                1,
                SpriteEffects.None
                );
            //GameShaders.Misc["QueenSlime"].Apply(data);

            Main.EntitySpriteDraw(
                data
            );

            SpriteBatchExt.Restart(Main.spriteBatch, prevState);

            // Draw crown
            Vector2 crownOffset = new Vector2(0, Projectile.height / 2f).RotatedBy(Projectile.rotation);
            Main.EntitySpriteDraw(
                crown.Value,
                Projectile.Center - crownOffset - Main.screenPosition,
                crown.Frame(),
                Color.White,
                Projectile.rotation,
                crown.Size() / 2f,
                1,
                SpriteEffects.None
            );

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
    }
}
