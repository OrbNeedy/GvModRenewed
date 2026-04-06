using GvMod.Common.Players;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    // Notes: During daytime, takes double contact damage (Reduction to timeLeft) and deals 25%
    // less damage rather than despawning
    class EyeOfCthulhu : ModProjectile
    {
        public Vector2 targetPosition = Vector2.Zero;
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int State { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Timer { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int DashCount { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public float StoredRotation { get => Projectile.localAI[1]; set => Projectile.localAI[1] = value; }
        private int minFrame = 0;
        private int maxFrame = 3;
        // Frame, Position, Rotation, Time Left
        private List<(int, Vector2, float, int)> AfterimageData = new();

        public override void SetDefaults()
        {
            Projectile.width = (int)TextureAssets.Npc[NPCID.EyeofCthulhu].Size().X;
            Projectile.height = (int)(TextureAssets.Npc[NPCID.EyeofCthulhu].Size().Y / 6);
            Main.projFrames[Projectile.type] = 6;
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

        public override string Texture => $"Terraria/Images/NPC_{NPCID.EyeofCthulhu}";

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(targetPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetPosition = reader.ReadVector2();
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (State == 4)
            {
                minFrame = 3;
                maxFrame = 6;
            }

            SoundEngine.PlaySound(SoundID.ForceRoar, Projectile.Center);
        }

        public override void AI()
        {
            Vector2 newVelocity = Vector2.Zero;
            float lerpAmount = 0.08f;
            bool allowDespawnSwitch = false;
            bool allowAfterimage = false;

            TargetingSystem();
            
            if (State != -1)
            {
                if (TargetNPC == -1)
                {
                    //Main.NewText("No target", new Color(255, 0, 0));

                    float idleSpeed = 8;
                    int ownerDirection = Main.player[Projectile.owner].direction;
                    Vector2 idlePosition = Main.player[Projectile.owner].Center + new Vector2(-48 * ownerDirection, -144);

                    if (Projectile.Center.Distance(idlePosition) >= 900)
                    {
                        idleSpeed *= 1 + (Projectile.Center.Distance(idlePosition) / 900);
                        lerpAmount = 0.2f;
                    }

                    if (Projectile.Center.Distance(idlePosition) < 16)
                    {
                        idleSpeed /= 2;
                        lerpAmount = 0.04f;
                    }

                    Projectile.rotation = Projectile.Center.
                        DirectionTo(targetPosition).
                        ToRotation() - MathHelper.PiOver2;
                    newVelocity = Projectile.Center.
                        DirectionTo(idlePosition) * idleSpeed;
                    
                    allowDespawnSwitch = true;
                }
                else
                {
                    //Main.NewText("Target available", new Color(0, 255, 0));

                    switch (State)
                    {
                        case 0: // Trailing behavior
                            float addedRotation = 0;
                            if(Projectile.timeLeft <= 1800)
                            {
                                lerpAmount = 0.06f;
                                addedRotation = 0 + ((float)(1800 - Projectile.timeLeft) * 0.0333f);
                                Projectile.rotation += addedRotation;

                                if (Projectile.timeLeft % 5 == 0 && Main.myPlayer == Projectile.owner)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                        new Vector2(0, 1).RotatedBy(Projectile.rotation) * 8,
                                        ModContent.ProjectileType<CthulhuServant>(), Projectile.damage / 2,
                                        Projectile.knockBack * 0.5f, Projectile.owner, TargetNPC, Projectile.whoAmI);
                                }

                                if (addedRotation >= 2)
                                {
                                    State = 2;
                                    Timer = 0;

                                    minFrame = 3;
                                    maxFrame = 6;

                                    Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
                                    SoundEngine.PlaySound(SoundID.ForceRoar, Projectile.Center);
                                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, 
                                        goreVel.RotatedByRandom(MathHelper.TwoPi), 8);
                                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                                        goreVel.RotatedByRandom(MathHelper.TwoPi), 8);
                                }
                                break;
                            }

                            lerpAmount = 0.08f;
                            newVelocity = Projectile.Center.
                                DirectionTo(targetPosition + new Vector2(0, -320)) * 8;

                            if (Timer % 90 == 0 && Main.myPlayer == Projectile.owner)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 
                                    Projectile.Center.DirectionTo(targetPosition) * 4, 
                                    ModContent.ProjectileType<CthulhuServant>(), Projectile.damage / 2, 
                                    Projectile.knockBack * 0.5f, Projectile.owner, TargetNPC, Projectile.whoAmI);
                            }

                            if (Timer >= 300)
                            {
                                State = 1;
                            }

                            Projectile.rotation = Projectile.Center.
                                DirectionTo(targetPosition).
                                ToRotation() - MathHelper.PiOver2;

                            allowDespawnSwitch = true;
                            break;
                        case 1: // Standard dash behavior
                            if (Timer >= 90)
                            {
                                Timer = 0;
                                if (DashCount >= 3)
                                {
                                    State = 0;
                                    DashCount = 0;
                                    break;
                                }

                                newVelocity = Projectile.Center.DirectionTo(targetPosition) * 560;
                                lerpAmount = 1; 
                                Projectile.rotation = newVelocity.ToRotation() - MathHelper.PiOver2;
                                DashCount++;

                                SoundEngine.PlaySound(SoundID.ForceRoar, Projectile.Center);
                            }

                            lerpAmount = 0.025f;
                            break;
                        case 2: // Low health trailing behavior
                            addedRotation = 2 - ((float)(1740 - Projectile.timeLeft) * 0.0666f);
                            if (addedRotation > 0)
                            {
                                lerpAmount = 0.08f;
                                Projectile.rotation += addedRotation;
                                break;
                            }

                            lerpAmount = 0.065f;
                            newVelocity = Projectile.Center.
                                DirectionTo(targetPosition + new Vector2(0, -320)) * 10;

                            if (Timer >= 210)
                            {
                                State = 3;
                            }

                            Projectile.rotation = Projectile.Center.
                                DirectionTo(targetPosition).
                                ToRotation() - MathHelper.PiOver2;

                            if (Projectile.timeLeft <= 600)
                            {
                                allowAfterimage = true;
                            }
                            allowDespawnSwitch = true;
                            break;
                        case 3: // Low health dashing behavior
                            float maxDashTimer = 90 - ((1800 - Projectile.timeLeft) * 0.04f);
                            float maxDashCount = 3 + ((1800 - Projectile.timeLeft) * 0.003f);
                            if (Timer >= maxDashTimer)
                            {
                                Timer = 0;
                                if (DashCount >= maxDashCount)
                                {
                                    State = 2;
                                    DashCount = 0;
                                    break;
                                }

                                newVelocity = Projectile.Center.DirectionTo(targetPosition) * 800;
                                lerpAmount = 1;
                                Projectile.rotation = newVelocity.ToRotation() - MathHelper.PiOver2;
                                DashCount++;

                                SoundEngine.PlaySound(SoundID.ForceRoarPitched, Projectile.Center);
                            }

                            allowAfterimage = true;
                            lerpAmount = 0.0425f;
                            break;
                    }
                }
            } else
            {
                if (Projectile.Center.X > 0)
                {
                    Projectile.timeLeft++;
                }

                newVelocity = new Vector2(0, -64);
                lerpAmount = 0.00175f;

                Projectile.rotation = Projectile.Center.
                        DirectionTo(targetPosition).
                        ToRotation() - MathHelper.PiOver2;

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

            if (allowAfterimage && Projectile.timeLeft % 4 == 0)
            {
                AfterimageData.Add((Projectile.frame, Projectile.Center, Projectile.rotation, 10));
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, newVelocity, lerpAmount);

            Timer++;
            ControlAnimations();
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
            } else
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
                } else
                {
                    targetPosition = Main.npc[TargetNPC].Center;
                }
            }
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
            if (Projectile.frameCounter >= 15)
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

            for (int i = 0; i < AfterimageData.Count; i++)
            {
                AfterimageData[i] = (AfterimageData[i].Item1, AfterimageData[i].Item2, 
                    AfterimageData[i].Item3, AfterimageData[i].Item4 - 1);

                if (AfterimageData[i].Item4 <= 0)
                {
                    AfterimageData.RemoveAt(i);
                    i--;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (State == 3 || State == 4)
            {
                modifiers.SourceDamage += 1.5f;
                modifiers.Knockback += 1.75f;
            }
            if (Main.dayTime)
            {
                modifiers.SourceDamage -= 0.25f;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (State == 3 || State == 4)
            {
                modifiers.SourceDamage += 1.5f;
                modifiers.Knockback += 1.75f;
            }
            if (Main.dayTime)
            {
                modifiers.SourceDamage -= 0.25f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft -= 4;

            if (Main.dayTime)
            {
                Projectile.timeLeft -= 4;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 8;

            if (Main.dayTime)
            {
                Projectile.timeLeft -= 8;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
            Vector2 additionalVel = Projectile.velocity * 0.9f;

            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 9);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 10);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 10);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> asset = TextureAssets.Npc[NPCID.EyeofCthulhu];
            Rectangle bounds = asset.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

            Main.EntitySpriteDraw(
                asset.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                lightColor,
                Projectile.rotation,
                new Vector2(bounds.Width * 0.5f, bounds.Height * 0.6f),
                1,
                SpriteEffects.None
            );
            return false;
        }

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> asset = TextureAssets.Npc[NPCID.EyeofCthulhu];
            Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            foreach ((int, Vector2, float, int) data in AfterimageData)
            {
                // Frame, Position, Rotation, Time Left
                Rectangle bounds = asset.Frame(1, Main.projFrames[Projectile.type], 0, data.Item1);
                Main.EntitySpriteDraw(
                    asset.Value, 
                    data.Item2 - Main.screenPosition,
                    bounds, 
                    lightColor * 0.25f, 
                    data.Item3,
                    new Vector2(bounds.Width * 0.5f, bounds.Height * 0.6f),
                    1, 
                    SpriteEffects.None
                );
            }
            return base.PreDrawExtras();
        }
    }
}
