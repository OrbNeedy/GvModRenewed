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
    class Sharkron : ModProjectile
    {
        public Vector2 targetPosition = Vector2.Zero;
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int Direction { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int TornadoHeight { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public bool Launched { get => Projectile.localAI[0] == 1; set => Projectile.localAI[0] = value ? 1 : 0; }
        public int Timer { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        bool drawAfterimages = false;

        public override void SetDefaults()
        {
            Projectile.width = (int)TextureAssets.Npc[NPCID.Sharkron].Size().X;
            Projectile.height = (int)(TextureAssets.Npc[NPCID.Sharkron].Size().Y / 4);
            Main.projFrames[Projectile.type] = 4;
            Projectile.frame = 0;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 600; // 3600
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/NPC_{NPCID.Sharkron}";

        public override void OnSpawn(IEntitySource source)
        {
            TargetingSystem();
        }

        public override void AI()
        {
            float tornadoTime = 60f;
            int additionalSinePeaks = 2; // Negative numbers become valleys
            float peakMult = 1 + (additionalSinePeaks * 4);
            float waveMult = MathF.Asin(-1) / tornadoTime;
            float sineVal = MathF.Sin(Timer * waveMult * peakMult * Direction);

            if (Timer <= tornadoTime)
            {
                TargetingSystem();

                Direction = Projectile.Center.DirectionTo(targetPosition).X > 0 ? 1 : -1;
                Projectile.rotation = new Vector2(Direction * 5, -2).ToRotation();

                Projectile.velocity.Y = -TornadoHeight * 0.8f;
                Projectile.velocity.X = sineVal * 6f;

                if (Timer == tornadoTime)
                {
                    Vector2 targetVelocity = Projectile.Center.DirectionTo(targetPosition);
                    targetVelocity *= 16;

                    Projectile.velocity = targetVelocity;
                    Projectile.rotation = targetVelocity.ToRotation();
                }
            } else
            {
                Projectile.velocity.Y += 0.08f;
                Projectile.velocity.Y *= 0.999f;

                Projectile.rotation = Projectile.velocity.ToRotation();
                drawAfterimages = true;
            }

            if (Projectile.rotation > MathHelper.PiOver2 || Projectile.rotation < -MathHelper.PiOver2)
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation -= MathHelper.Pi;
            }
            else
            {
                Projectile.spriteDirection = -1;
            }

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
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            //SoundEngine.PlaySound(SoundID.NPCDeath20, Projectile.Center);
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
            Vector2 additionalVel = Projectile.velocity * 0.9f;

            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 577);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 578);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 579);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 583);
        }

        public override bool PreDrawExtras()
        {
            if (drawAfterimages)
            {
                Asset<Texture2D> asset = TextureAssets.Npc[NPCID.Sharkron];
                Rectangle bounds = asset.Frame(1, 8, 0, Projectile.frame);
                Vector2 offset = Projectile.Size / 2;
                Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
                for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
                {
                    float transparency = 1f - ((float)i / (float)ProjectileID.Sets.TrailCacheLength[Type]);
                    //Main.NewText($"Transparency {i}: {transparency}");
                    Main.EntitySpriteDraw(
                        asset.Value,
                        Projectile.oldPos[i] + offset - Main.screenPosition,
                        bounds,
                        lightColor * 0.75f * transparency * Projectile.Opacity,
                        Projectile.oldRot[i],
                        bounds.Size() / 2,
                        1,
                        Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
                    );
                }
            }
            return base.PreDrawExtras();
        }

        public override void PostDraw(Color lightColor)
        {
            /*Asset<Texture2D> asset = TextureAssets.DukeFishron;
            Rectangle bounds = asset.Frame(1, 8, 0, Projectile.frame);
            Main.EntitySpriteDraw(
                asset.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                Color.White,
                Projectile.rotation,
                bounds.Size() / 2,
                1,
                Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
            );*/
            base.PostDraw(lightColor);
        }
    }
}
