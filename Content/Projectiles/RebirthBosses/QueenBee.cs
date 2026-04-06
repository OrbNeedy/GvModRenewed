using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using GvMod.Common.Players;
using GvMod.Common.Utils;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using System.IO;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class QueenBee : ModProjectile
    {
        public Vector2 targetPosition = Vector2.Zero;
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int State { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Timer { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int DashCount { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        private int minFrame = 0;
        private int maxFrame = 4;

        public override void SetDefaults()
        {
            Projectile.width = (int)TextureAssets.Npc[NPCID.QueenBee].Size().X;
            Projectile.height = (int)(TextureAssets.Npc[NPCID.QueenBee].Size().Y / 12);
            Main.projFrames[Projectile.type] = 12;
            Projectile.frame = 0;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600; // 3600
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/NPC_{NPCID.QueenBee}";

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(targetPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            TargetingSystem();
            Vector2 newVelocity = Vector2.Zero;
            float lerpAmount = 0.04f;
            float xOffset = 0;
            float yOffset = 0;
            bool allowDespawnSwitch = false;

            if (State != -1)
            {
                if (TargetNPC != -1)
                {
                    switch (State)
                    {
                        case 0: // Dash 
                            if (Projectile.Center.Y <= targetPosition.Y + 32 &&
                                Projectile.Center.Y >= targetPosition.Y - 32 &&
                                Projectile.Center.Distance(targetPosition) <= 352)
                            {
                                // Sound: Zombie125
                                newVelocity = Projectile.Center.DirectionTo(targetPosition) * 64;
                                lerpAmount = 1;

                                DashCount++;
                                State = 1;
                                Timer = 0;

                                minFrame = 0;
                                maxFrame = 4;
                            }
                            else
                            {
                                Projectile.spriteDirection = Projectile.DirectionTo(targetPosition).X < 0 ? 1 : -1;
                                Vector2 dashPositionOffset = new Vector2(DashCount % 2 == 0 ? 320 : -320, 0);
                                float searchSpeed = 8;
                                lerpAmount = 0.1f;

                                if (Projectile.timeLeft <= 1800)
                                {
                                    searchSpeed = 12;
                                    lerpAmount = 0.12f;
                                }

                                newVelocity = Projectile.Center.DirectionTo(targetPosition +
                                    dashPositionOffset) * searchSpeed;
                            }
                            break;
                        case 1:
                            int maxDash = 3;
                            float speedLimit = 8;
                            int maxTime = 90;

                            if (Projectile.timeLeft <= 1800)
                            {
                                maxDash = 6;
                                speedLimit = 12;
                                maxTime = 45;
                            }

                            if (Timer >= maxTime || Projectile.velocity.Length() <= speedLimit)
                            {
                                if (DashCount >= maxDash)
                                {
                                    State = Main._rand.Next(2, 4);
                                    DashCount = 0;
                                    Projectile.netUpdate = true;
                                }
                                else
                                {
                                    State = 0;
                                }
                                Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
                                Timer = 0;
                                minFrame = 4;
                                maxFrame = Main.projFrames[Projectile.type];
                            }
                            lerpAmount = 0.08f;
                            break;
                        case 2: // Shoot bees
                        case 3: // Shoot stinger
                            xOffset = MathF.Sin(Timer * 0.02f) * 320;
                            yOffset = MathF.Abs(MathF.Cos(Timer * 0.02f)) * -320;//(0.5f + (MathF.Cos(Timer * 0.04f) * 0.5f)) * 160; 
                            newVelocity = Projectile.Center.DirectionTo(targetPosition +
                                new Vector2(xOffset, yOffset)) * 8;
                            Projectile.spriteDirection = Projectile.DirectionTo(targetPosition).X < 0 ? 1 : -1;

                            int shootingTime = 60;
                            if (Projectile.timeLeft <= 1800)
                            {
                                shootingTime = 30;
                            }

                            if (Timer % shootingTime == 0)
                            {
                                if (State == 2)
                                {
                                    SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                                    int id = Main._rand.Next([ProjectileID.Bee, ProjectileID.GiantBee]);
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                        Projectile.Center.DirectionTo(targetPosition) * 1, id,
                                        (int)(Projectile.damage * 0.15f), 1, Projectile.owner);
                                }
                                else
                                {
                                    SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                        Projectile.Center.DirectionTo(targetPosition) * 10,
                                        ModContent.ProjectileType<Stinger>(),
                                        (int)(Projectile.damage * 0.5f), 1, Projectile.owner);
                                }
                            }
                            allowDespawnSwitch = true;

                            if (Timer >= 480)
                            {
                                State = 0;
                                Timer = 0;
                            }
                            break;
                    }
                }
                else
                {
                    // Follow player
                    Vector2 direction = Projectile.Center.DirectionTo(Main.player[Projectile.owner].Center);
                    xOffset = MathF.Sin(Timer * 0.02f) * 320;
                    yOffset = MathF.Abs(MathF.Cos(Timer * 0.02f)) * -320;//(0.5f + (MathF.Cos(Timer * 0.04f) * 0.5f)) * 160; 
                    newVelocity = Projectile.Center.DirectionTo(Main.player[Projectile.owner].Center +
                        new Vector2(xOffset, yOffset)) * 8;
                    minFrame = 4;
                    maxFrame = Main.projFrames[Projectile.type];
                    Projectile.spriteDirection = direction.X < 0 ? 1 : -1;
                    allowDespawnSwitch = true;
                }
            } else
            {
                if (Projectile.Center.X > 0)
                {
                    Projectile.timeLeft++;
                }

                newVelocity = new Vector2(0, -64);
                lerpAmount = 0.00175f;

                Projectile.spriteDirection = Projectile.DirectionTo(targetPosition).X < 0 ? 1 : -1;

                if (Timer >= 600)
                {
                    Projectile.timeLeft = -2;
                }
            }

            if (Projectile.timeLeft <= 3)
            {
                if (allowDespawnSwitch)
                {
                    Timer = 0;
                    State = -1;
                }

                Projectile.timeLeft++;
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
            if (Projectile.frameCounter >= 2)
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft -= 4;
            target.AddBuff(BuffID.Poisoned, 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 8;
            target.AddBuff(BuffID.Poisoned, 180);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath66, Projectile.Center);
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
            Vector2 additionalVel = Projectile.velocity * 0.9f;

            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 303);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 304);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 305);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 306);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 307);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 308);
        }
    }
}
