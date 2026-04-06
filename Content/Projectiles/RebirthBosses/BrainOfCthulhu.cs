using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GvMod.Common.Players;
using GvMod.Common.Utils;
using Terraria.Audio;
using System.Collections.Generic;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class BrainOfCthulhu : ModProjectile
    {
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int State { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Timer { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        List<int> Creepers = new();
        Vector2 targetPosition = Vector2.Zero;
        int minFrame = 0;
        int maxFrame = 4;

        public override void SetDefaults()
        {
            Projectile.width = (int)(TextureAssets.Npc[NPCID.BrainofCthulhu].Size().X);
            Projectile.height = (int)(TextureAssets.Npc[NPCID.BrainofCthulhu].Size().Y / 8);
            Main.projFrames[Projectile.type] = 8;
            Projectile.frame = 0;
            Projectile.Opacity = 0;
            //Projectile.light = 0.5f;

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

        public override string Texture => $"Terraria/Images/NPC_{NPCID.BrainofCthulhu}";

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2(Main._rand.NextFloat(1, 32)).RotatedByRandom(MathHelper.TwoPi);
                    // Give all creepers an offset and variation value so they will swarm more naturally
                    float randVar = Main._rand.NextFloat(0, 1);
                    int projIndex = Projectile.NewProjectile(
                        source, Projectile.Center + offset, Vector2.Zero, 
                        ModContent.ProjectileType<Creeper>(), (int)(Projectile.damage * 0.65f), 
                        1.5f, Projectile.owner, TargetNPC, randVar, Projectile.whoAmI
                        );
                    Creepers.Add(projIndex);
                }
            }
        }

        public override void AI()
        {
            TargetingSystem();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Creepers.Count; i++)
                {
                    //Main.NewText("Creeper inactive, deleting from list");
                    int projIndex = Creepers[i];
                    if (projIndex != -1)
                    {
                        Projectile proj = Main.projectile[projIndex];
                        if (!proj.active || proj.owner != Projectile.owner ||
                            proj.ModProjectile is not Creeper)
                        {
                            //Main.NewText("Creeper inactive, deleting from list");
                            Creepers.RemoveAt(i);
                            i--;
                            continue;
                        }
                        else
                        {
                            if (proj.ai[0] != TargetNPC)
                            {
                                proj.ai[0] = TargetNPC;
                                proj.netUpdate = true;
                            }
                        }
                    }
                }
            }

            if (TargetNPC == -1 )
            {
                targetPosition = Main.player[Projectile.owner].Center;
            }

            if (Projectile.timeLeft <= 3 && State != -1)
            {
                State = -1;
                Timer = 0;
            }

            switch (State)
            {
                case -1:
                    Projectile.timeLeft++;
                    DespawnBehavior();
                    break;
                case 0: // Phase 1
                    Phase1AI();
                    break;
                case 1: // Phase 2
                    Phase2AI();
                    break;
            }

            Projectile.Opacity = float.Clamp(Projectile.Opacity, 0, 1);

            ControlAnimations();
        }

        public void DespawnBehavior()
        {
            Projectile.velocity.Y += 0.6f;

            Projectile.Opacity -= 1f / 60f;
            if (Projectile.Opacity <= 0 || Timer >= 600 || 
                Projectile.Center.Y >= Main.maxTilesY)
            {
                Projectile.timeLeft = -1;
                Projectile.active = false;
                return;
            }
        }

        public void Phase1AI()
        {
            if (Projectile.Center == targetPosition) return;

            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);

            if (TargetNPC == -1)
            {
                Vector2 offset = new Vector2(0, -320);
                direction = Projectile.Center.DirectionTo(targetPosition + offset);

                if (Projectile.Center.Distance(targetPosition + offset) >= 800)
                {
                    Timer = 200;
                } else
                {
                    Timer = 0;
                }
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 4, 0.2f);

            if (Timer >= 200)
            {
                Projectile.Opacity -= 1f / 40f;
                if (Projectile.Opacity <= 0)
                {
                    Projectile.Center = targetPosition + new Vector2(640, 0).RotatedByRandom(MathHelper.TwoPi);
                    Timer = 0;
                    Projectile.netUpdate = true;
                }
            } else
            {
                if (Creepers.Count <= 0 && Projectile.Opacity >= 1)
                {
                    Timer = 0;
                    State = 1;

                    minFrame = 4;
                    maxFrame = 8;
                    // Add gores
                }

                Projectile.Opacity += 1f / 40f;
                Timer++;
            }
        }

        public void Phase2AI()
        {
            if (Projectile.Center == targetPosition) return;

            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);

            if (TargetNPC == -1)
            {
                Vector2 offset = new Vector2(0, -320);
                direction = Projectile.Center.DirectionTo(targetPosition + offset);

                if (Projectile.Center.Distance(targetPosition + offset) >= 800)
                {
                    Timer = 100;
                }
                else
                {
                    Timer = 0;
                }
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 4, 0.15f);

            if (Timer >= 100)
            {
                Projectile.Opacity -= 1f / 20f;
                if (Projectile.Opacity <= 0)
                {
                    Projectile.Center = targetPosition + new Vector2(256, 0).RotatedByRandom(MathHelper.TwoPi);
                    Timer = 0;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                if (Creepers.Count <= 0)
                {
                    Timer = 0;
                    State = 1;

                    minFrame = 4;
                    maxFrame = 8;
                    // Add gores
                }

                Projectile.Opacity += 1f / 20f;
                Timer++;
            }
        }

        public void TargetingSystem()
        {
            float maxDistance = 3200;

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
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (State == 0) return false;
            return base.CanHitNPC(target);
        }

        public override bool CanHitPlayer(Player target)
        {
            if (State == 0) return false;
            return base.CanHitPlayer(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft -= 2;
            if (Main._rand.NextBool(3))
            {
                int buffType = Main._rand.Next([BuffID.Poisoned, BuffID.Bleeding, 
                    BuffID.Confused, BuffID.BrokenArmor]);
                target.AddBuff(buffType, 150);
            }
            // Poisoned, bleeding, confused, broken armor
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 4;
            if (Main._rand.NextBool(3))
            {
                int buffType = Main._rand.Next([BuffID.Poisoned, BuffID.Darkness, 
                    BuffID.Cursed, BuffID.Bleeding, BuffID.Confused, BuffID.Slow, 
                    BuffID.Weak, BuffID.Silenced, BuffID.BrokenArmor]);
                target.AddBuff(buffType, 120);
            }
        }

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> texture = TextureAssets.Npc[NPCID.BrainofCthulhu];
            Rectangle bounds = texture.Frame(1, 8, 0, Projectile.frame);
            float transparency = float.Clamp((1800f - Projectile.timeLeft) / 1320f, 0, 1);
            Vector2 offsetToTarget = Projectile.Center - targetPosition;
            for (int i = 1; i < 4; i++)
            {
                Vector2 finalPosition = targetPosition + offsetToTarget * 
                    new Vector2(1, 1).RotatedBy(MathHelper.PiOver2 * i);
                Main.EntitySpriteDraw(
                    texture.Value,
                    finalPosition - Main.screenPosition + (Projectile.Size / 2f),
                    bounds, 
                    Lighting.GetColor(finalPosition.ToTileCoordinates()) * transparency, 
                    Projectile.rotation, 
                    bounds.Size(), 
                    Projectile.scale, 
                    SpriteEffects.None
                    );
                // Draw copies
            }

            return base.PreDrawExtras();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
            Vector2 additionalVel = Projectile.velocity * 0.9f;

            /*Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 1258);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 1259);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 1259);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 383);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 384);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 385);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 386);*/
        }
    }
}
