using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.Audio;
using GvMod.Common.Players;
using GvMod.Common.Utils;
using System;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class Skeletron : ModProjectile
    {
        public Vector2 targetPosition = Vector2.Zero;
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int State { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int SkullShooting { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int Timer { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public int SkullTimer { get => (int)Projectile.localAI[1]; set => Projectile.localAI[1] = value; }
        public static int ApproachTime = 600;

        public override void SetDefaults()
        {
            Projectile.width = (int)TextureAssets.Npc[NPCID.SkeletronHead].Size().X;
            Projectile.height = (int)(TextureAssets.Npc[NPCID.SkeletronHead].Size().Y);
            Main.projFrames[Projectile.type] = 1;
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

        public override string Texture => $"Terraria/Images/NPC_{NPCID.SkeletronHead}";

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
            // Spawn both hands regardless of state
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<SkeletronArm>(), Projectile.damage, Projectile.knockBack * 2, 
                Projectile.owner, Projectile.whoAmI, 1);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<SkeletronArm>(), Projectile.damage, Projectile.knockBack * 2,
                Projectile.owner, Projectile.whoAmI, -1);

            SoundEngine.PlaySound(SoundID.ForceRoar, Projectile.Center);
        }

        public override void AI()
        {
            Vector2 newVelocity = Vector2.Zero;
            float lerpAmount = 0.04f;
            bool allowDespawnSwitch = false;

            TargetingSystem();

            switch (State)
            {
                case -1: // Radiant Fetters' use case
                    break;
                case 0: // Regular use case
                    // Trailing behavior
                    newVelocity = Projectile.Center.
                        DirectionTo(targetPosition + new Vector2(MathF.Cos(Timer * 0.02f) * 320, -320)) * 6;

                    Projectile.rotation = Projectile.velocity.X * 0.08f;

                    if (Timer >= ApproachTime && TargetNPC != -1)
                    {
                        Timer = 0;
                        State = 1;
                        SoundEngine.PlaySound(SoundID.ForceRoar, Projectile.Center);
                    }
                    allowDespawnSwitch = true;
                    // Shoot projectiles 
                    if (SkullShooting > 0 && TargetNPC != -1 && Timer > 0)
                    {
                        float skullTime = 60f / (float)SkullShooting;
                        if ((float)Timer % skullTime == 0)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 
                                Projectile.DirectionTo(targetPosition) * 0.2f, 
                                ModContent.ProjectileType<SkullProjectile>(), Projectile.damage / 3, 2, 
                                Projectile.owner, TargetNPC);
                        }
                    }
                    break;
                case 1:
                    // Beeline to the enemy
                    newVelocity = Projectile.Center.DirectionTo(targetPosition) * 8;
                    lerpAmount = 0.1f;
                    Projectile.rotation -= 0.35f;
                    if (Timer > ApproachTime / 2 && Projectile.timeLeft > 600)
                    {
                        Timer = 0;
                        State = 0;
                    }
                    // On the last 10 seconds, don't stop spinning and increase approaching speed
                    if (Projectile.timeLeft <= 600)
                    {
                        allowDespawnSwitch = true;
                        lerpAmount = 0.125f;
                    }
                    break;
                case 2:
                    // Despawn
                    newVelocity = new Vector2(0, 64);
                    lerpAmount = 0.01f;

                    if (Projectile.Center.Y > Main.maxTilesY * 16 || Timer >= 600)
                    {
                        Projectile.timeLeft = -1;
                        return;
                    }
                    break;
            }

            Vector2 dustVel = new Vector2(0, 1 + (Main._rand.NextFloat() * 3)).
                RotatedBy(Projectile.rotation).
                RotatedByRandom(MathHelper.PiOver4 / 2);
            Vector2 dustPosition = Projectile.Center + new Vector2(0, Projectile.height * 0.425f).
                RotatedBy(Projectile.rotation);
            Dust.NewDust(dustPosition, 0, 0, DustID.Blood, dustVel.X, dustVel.Y, Scale: 2);

            if (Projectile.timeLeft <= 3)
            {
                if (allowDespawnSwitch)
                {
                    Timer = 0;
                    State = 2;
                }

                Projectile.timeLeft++;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, newVelocity, lerpAmount);

            Timer++;
            SkullTimer++;
            if (SkullTimer >= 1200)
            {
                SkullShooting++;
                SkullTimer = 0;
            }
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (State == 1)
            {
                modifiers.SourceDamage += 1.5f;
                modifiers.Knockback += 1.75f;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (State == 1)
            {
                modifiers.SourceDamage += 1.5f;
                modifiers.Knockback += 1.75f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft -= 4;
            SkullTimer += 4;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 8;
            SkullTimer += 8;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);

            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi), 54);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi), 55);
        }
    }
}
