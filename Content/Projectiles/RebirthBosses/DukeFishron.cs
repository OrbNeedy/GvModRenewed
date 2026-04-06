using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GvMod.Common.Players;
using GvMod.Common.Utils;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using System;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class DukeFishron : ModProjectile
    {
        public Vector2 targetPosition = Vector2.Zero;
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int State { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Timer { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int DashCount { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public int StateCounter { get => (int)Projectile.localAI[1]; set => Projectile.localAI[1] = value; }
        public bool showAfterimages = false;
        public float afterimageOffset = 0;
        public float eyeglowOpacity = 0f;
        public float intendedRotation = 0f;
        private int minFrame = 0;
        private int maxFrame = 6;

        public override void SetDefaults()
        {
            // Main.instance.LoadNPC(NPCID.DukeFishron);
            Projectile.width = (int)TextureAssets.Npc[NPCID.DukeFishron].Size().X;
            Projectile.height = (int)(TextureAssets.Npc[NPCID.DukeFishron].Size().Y / 8);
            Main.projFrames[Projectile.type] = 8;
            Projectile.frame = 0;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;

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

        public override string Texture => $"Terraria/Images/NPC_{NPCID.DukeFishron}";

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0;
            intendedRotation = Projectile.rotation;

            int particles = 50;
            Vector2 vel = new Vector2(0, 4);
            float rotationPerIteration = MathHelper.TwoPi / (float)particles;
            for (int i = 0; i < particles; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.FishronWings, 
                    vel.RotatedBy(i * rotationPerIteration));
            }
        }

        public override void AI()
        {
            TargetingSystem();
            Vector2 newVelocity = Vector2.Zero;
            float lerpAmount = 0.12f;
            bool despawnAllow = false;
            float targetOpacity = 1;
            float targetEyeglowOpacity = 0;
            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);

            /*Main.NewText("\nState: " + State, Color.Blue);
            Main.NewText("Timer: " + Timer, Color.Blue);
            Main.NewText("Dash counter: " + DashCount, Color.Blue);
            Main.NewText("State Counter: " + StateCounter, Color.Blue);
            Main.NewText("Opacity: " + Projectile.Opacity, Color.Blue);
            Main.NewText("Target opacity: " + targetOpacity, Color.Blue);
            Main.NewText("Eye glow opacity: " + eyeglowOpacity, Color.Blue);*/

            if (TargetNPC != -1)
            {
                switch (State)
                {
                    case 0: // Normal behavior (Dashing)
                        despawnAllow = true;
                        showAfterimages = true;

                        maxFrame = 6;
                        minFrame = 0;

                        if (Projectile.timeLeft <= 2400)
                        {
                            targetEyeglowOpacity = 1;
                            if (eyeglowOpacity < 1)
                            {
                                Timer = 0;
                                break;
                            }
                        }

                        if (Projectile.timeLeft <= 1200)
                        {
                            Timer = -16;
                            State = 3;
                            StateCounter = 0;
                            DashCount = 0;
                        }

                        if (Projectile.Opacity < 1)
                        {
                            Timer = 0;
                        }

                        if (Timer >= 20)
                        {
                            if (Timer == 20)
                            {
                                intendedRotation = direction.ToRotation();

                                newVelocity = direction * 18;
                                lerpAmount = 1;
                                DashCount++;
                            } else
                            {
                                lerpAmount = 0.01f;

                                for (int i = 0; i < 5; i++)
                                {
                                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                                        DustID.FishronWings, Projectile.velocity.X * 0.5f,
                                        Projectile.velocity.Y * 0.5f);
                                }
                                
                                maxFrame = Main.projFrames[Projectile.type];
                                minFrame = Main.projFrames[Projectile.type] - 1;
                                despawnAllow = false;
                            }

                            if (Timer >= 60)
                            {
                                Timer = 0;
                                if (DashCount >= 5)
                                {
                                    if (StateCounter % 2 == 0)
                                    {
                                        State = 1;
                                    } else
                                    {
                                        State = 2;
                                    }
                                    DashCount = 0;
                                    StateCounter++;
                                }
                            }
                        } else
                        {
                            float targetRotation = Projectile.Center.DirectionTo(targetPosition).ToRotation();
                            newVelocity = new Vector2(0, -4);

                            intendedRotation = intendedRotation.AngleLerp(targetRotation, 0.04f);
                        }
                        break;
                    case 1: // Shoot whrirlpools
                        int sharknadoType = ModContent.ProjectileType<SharknadoSpawner>();
                        lerpAmount = 0.02f;
                        float whirlpoolMaxTimer = 60;

                        if (Timer >= whirlpoolMaxTimer / 2f && Timer < 3 * whirlpoolMaxTimer / 4f)
                        {
                            minFrame = 6;
                            maxFrame = 7;
                        } else
                        {
                            minFrame = 0;
                            maxFrame = 6;
                        }

                        if (Timer >= whirlpoolMaxTimer / 2f && Timer < 3 * whirlpoolMaxTimer / 4f)
                        {
                            float scale = (Timer - (whirlpoolMaxTimer / 2f)) / (whirlpoolMaxTimer / 4f);
                            afterimageOffset = MathF.Sin(scale * MathHelper.Pi);
                        }

                        if (Timer == whirlpoolMaxTimer / 2f)
                        {
                            // Sound effect
                            if (Projectile.timeLeft <= 2400)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                    direction * 8, sharknadoType, (int)(Projectile.damage * 1.3f), 0, 
                                    Projectile.owner, -1, 1);
                            } else
                            {
                                for (int i = -1; i < 2; i += 2)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                        direction.RotatedBy(i * MathHelper.PiOver2 / 3f) * 6, sharknadoType,
                                        Projectile.damage, 0, Projectile.owner);
                                }
                            }
                        }

                        if (Timer >= 60)
                        {
                            State = Projectile.timeLeft <= 1200 ? 3 : 0;
                            Timer = 0;
                        }

                        intendedRotation = direction.ToRotation();
                        break;
                    case 2: // Shoot bubbles
                        int bubbleType = ModContent.ProjectileType<Bubble>();
                        lerpAmount = 0.02f;
                        int minTimer = 15;
                        int maxTimer = 60;

                        if (Projectile.timeLeft <= 2400)
                        {
                            maxTimer = 75;
                        }

                        if (Timer >= minTimer && Timer <= maxTimer)
                        {
                            if (Projectile.timeLeft <= 2400)
                            {
                                lerpAmount = 1;
                                float timeScale = (Timer - minTimer) / (float)maxTimer;
                                newVelocity = new Vector2(16, 0).RotatedBy(MathHelper.Pi * timeScale * 4);
                                intendedRotation = Projectile.velocity.ToRotation();
                            }

                            minFrame = 6;
                            maxFrame = 7;

                            if (Timer % 5 == 0)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    Vector2 vel = direction.RotatedByRandom(MathHelper.PiOver2) * 6;
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                                        Projectile.Center, vel, bubbleType,
                                        (int)(Projectile.damage * 0.6f), 2, Projectile.owner);
                                }
                            }
                        }
                        else
                        {
                            minFrame = 0;
                            maxFrame = 6;
                        }

                        if (Timer >= maxTimer + 10)
                        {
                            State = Projectile.timeLeft <= 1200 ? 3 : 0;
                            Timer = 0;
                        }

                        intendedRotation = direction.ToRotation();
                        break;
                    case 3: // Phase 3 behavior (Dashing + teleporting)
                        despawnAllow = true;
                        showAfterimages = true;
                        targetEyeglowOpacity = 1;
                        int teleportDir = 1;
                        if (StateCounter % 2 == 0) teleportDir = -1;
                        Vector2 teleportOffset = new Vector2(288 * teleportDir, 0);

                        maxFrame = 6;
                        minFrame = 0;

                        if (Projectile.Opacity < 1 && targetOpacity < 1)
                        {
                            // Do not dash until Opacity is max
                            Timer = 0;
                        }

                        if (Timer == -15)
                        {
                            // Play sound 
                        }

                        if (Timer >= 15)
                        {
                            if (Timer == 15)
                            {
                                // At the start of the 15 frame mark, set velocity 
                                //Main.NewText("Dash velocity set", Color.Yellow);
                                intendedRotation = direction.ToRotation();

                                newVelocity = direction * 22;
                                lerpAmount = 1;
                                DashCount++;
                            }
                            else
                            {
                                // Start slowing down every other frame
                                lerpAmount = 0.01f;

                                for (int i = 0; i < 5; i++)
                                {
                                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                                        DustID.FishronWings, Projectile.velocity.X * 0.5f,
                                        Projectile.velocity.Y * 0.5f);
                                }

                                maxFrame = Main.projFrames[Projectile.type];
                                minFrame = Main.projFrames[Projectile.type] - 1;
                                despawnAllow = false;
                            }

                            if (Timer >= 30)
                            {
                                // After 30 frames, compare dash count with state count
                                if (DashCount >= StateCounter)
                                {
                                    //Main.NewText("DashCount surpassed StateCounter, turning transparent", Color.Yellow);
                                    // If dash count reached it's max, start teleporting by turning transparent
                                    if (Projectile.Opacity <= 0)
                                    {
                                        //Main.NewText("Fully transparent, teleporting", Color.Yellow);
                                        // When fully transparent, teleport and reset state 
                                        Timer = 0;
                                        
                                        StateCounter++;
                                        DashCount = 0;
                                        if (StateCounter > 3)
                                        {
                                            //Main.NewText("StateCounter reached max, resetting", Color.Yellow);
                                            StateCounter = 1;
                                        }

                                        // Change directions every so often
                                        Projectile.Center = targetPosition + teleportOffset;
                                    } else
                                    {
                                        //Main.NewText("Still trying to turn transparent", Color.Yellow);
                                        targetOpacity = 0;
                                        lerpAmount = 0.1f;
                                    }
                                } else
                                {
                                    //Main.NewText("DashCount not yet at StateCounter", Color.Yellow);
                                    Timer = 0;
                                }
                            }
                        }
                        else
                        {
                            // When not dashing, follow target
                            //Main.NewText("Not dashing", Color.Yellow);
                            float targetRotation = Projectile.Center.DirectionTo(targetPosition).ToRotation();
                            
                            newVelocity = Projectile.Center.DirectionTo(targetPosition + teleportOffset) * 6;

                            intendedRotation = intendedRotation.AngleLerp(targetRotation, 0.04f);
                        }
                        break;
                }
            } else
            {
                Player owner = Main.player[Projectile.owner];
                targetPosition = owner.Center;
                maxFrame = 6;
                minFrame = 0;

                Vector2 offset = new Vector2(-448 * owner.direction, -288);

                newVelocity = Projectile.Center.DirectionTo(targetPosition + offset) * 8;
                despawnAllow = true;
                targetOpacity = 1;
                showAfterimages = false;

                if (Projectile.Center.Distance(targetPosition) >= 1600)
                {
                    targetOpacity = 0;
                    if (targetOpacity >= 1)
                    {
                        Projectile.Center = targetPosition + 
                            new Vector2(0, 192).RotatedByRandom(MathHelper.TwoPi);
                    }
                }

                if (Projectile.Opacity < 1)
                {
                    newVelocity = new Vector2(0, -3);
                }

                float targetRotation = Projectile.Center.DirectionTo(targetPosition).ToRotation();

                intendedRotation = intendedRotation.AngleLerp(targetRotation, 0.06f);
                //Main.NewText("Rotation: " + Projectile.rotation);
            }

            if (State == -1)
            {
                Projectile.timeLeft++;

                lerpAmount = 0.08f;
                targetOpacity = 0;
                targetEyeglowOpacity = 0;
                newVelocity = new Vector2(0, -4);
                maxFrame = 6;
                minFrame = 0;

                if (Projectile.Opacity <= 0)
                {
                    if (eyeglowOpacity <= 0)
                    {
                        Projectile.timeLeft = -1;
                        Projectile.active = false;
                    }
                }
            } else
            {
                if (despawnAllow && Projectile.timeLeft <= 3)
                {
                    State = -1;
                    Projectile.timeLeft++;
                }
            }

            if (targetOpacity != Projectile.Opacity)
            {
                if (targetOpacity < Projectile.Opacity)
                {
                    Projectile.Opacity -= 1f / 60f;
                    if (Projectile.Opacity < targetOpacity)
                    {
                        Projectile.Opacity = targetOpacity;
                    }
                }
                else
                {
                    Projectile.Opacity += 1f / 60f;
                    if (Projectile.Opacity > targetOpacity)
                    {
                        Projectile.Opacity = targetOpacity;
                    }
                }
            }

            if (targetEyeglowOpacity != eyeglowOpacity)
            {
                if (targetEyeglowOpacity < eyeglowOpacity)
                {
                    eyeglowOpacity -= 1f / 150f;
                    if (eyeglowOpacity < targetEyeglowOpacity)
                    {
                        eyeglowOpacity = targetEyeglowOpacity;
                    }
                }
                else
                {
                    eyeglowOpacity += 1f / 150f;
                    if (eyeglowOpacity > targetEyeglowOpacity)
                    {
                        eyeglowOpacity = targetEyeglowOpacity;
                    }
                }
            }

            Projectile.rotation = intendedRotation;

            if (Projectile.rotation > MathHelper.PiOver2 || Projectile.rotation < -MathHelper.PiOver2)
            {
                //Main.NewText("Inversed", Color.Red);
                Projectile.spriteDirection = -1;
                Projectile.rotation -= MathHelper.Pi;
            }
            else
            {
                //Main.NewText("Normal", Color.Green);
                Projectile.spriteDirection = 1;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, newVelocity, lerpAmount);

            ControlAnimations();

            Timer++;
        }

        public void TargetingSystem()
        {
            float maxDistance = 1600;

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
            if (Projectile.frameCounter >= 4)
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
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath20, Projectile.Center);
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
            Vector2 additionalVel = Projectile.velocity * 0.9f;

            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 573);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 574);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 575);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 576);
        }

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> asset = TextureAssets.Npc[NPCID.DukeFishron];
            Rectangle bounds = asset.Frame(1, 8, 0, Projectile.frame);
            Vector2 offset = Projectile.Size / 2;
            Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            if (showAfterimages)
            {
                for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
                {
                    float transparency = 1f - ((float)i / (float)ProjectileID.Sets.TrailCacheLength[Type]);
                    //Main.NewText($"Transparency {i}: {transparency}");
                    Main.EntitySpriteDraw(
                        asset.Value,
                        Projectile.oldPos[i] + offset - Main.screenPosition,
                        bounds,
                        lightColor * 0.25f * transparency * Projectile.Opacity,
                        Projectile.oldRot[i],
                        bounds.Size() / 2,
                        1,
                        Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
                    );
                }
            }

            if (afterimageOffset > 0)
            {
                Color blueLightColor = lightColor;
                blueLightColor.R = 0;
                blueLightColor.G = 0;

                for (int i = 0; i < 3; i++)
                {
                    //Main.NewText($"Transparency {i}: {transparency}");
                    Vector2 specialOffset = new Vector2(0, 20 * afterimageOffset).RotatedBy(i * MathHelper.TwoPi / 3f);
                    Main.EntitySpriteDraw(
                        asset.Value,
                        Projectile.oldPos[i] + specialOffset + offset - Main.screenPosition,
                        bounds,
                        blueLightColor * 0.25f * Projectile.Opacity,
                        Projectile.oldRot[i],
                        bounds.Size() / 2,
                        1,
                        Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
                    );
                }
            }
            return base.PreDrawExtras();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft <= 2400)
            {
                lightColor.R = (byte)(lightColor.R * 0.65f);
                lightColor.G = (byte)(lightColor.G * 0.8f);
                lightColor.B = (byte)(lightColor.B * 0.65f);
            }

            return base.PreDraw(ref lightColor);
        }

        public override void PostDraw(Color lightColor)
        {
            Asset<Texture2D> asset = TextureAssets.DukeFishron;
            Rectangle bounds = asset.Frame(1, 8, 0, Projectile.frame);
            /*Main.EntitySpriteDraw(
                asset.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                Color.Yellow * eyeglowOpacity,
                Projectile.rotation,
                bounds.Size() / 2,
                1,
                Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
            );*/

            Vector2 offset = Projectile.Size / 2;
            for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            {
                float transparency = 1f - ((float)i / (float)ProjectileID.Sets.TrailCacheLength[Type]);
                //Main.NewText($"Transparency {i}: {transparency}");
                Main.EntitySpriteDraw(
                    asset.Value,
                    Projectile.oldPos[i] + offset - Main.screenPosition,
                    bounds,
                    Color.Yellow * 0.75f * transparency * eyeglowOpacity,
                    Projectile.oldRot[i],
                    bounds.Size() / 2,
                    1,
                    Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
                );
            }
            base.PostDraw(lightColor);
        }
    }
}
