using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using System;
using GvMod.Common.Players;
using GvMod.Common.Utils;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class SharknadoSpawner : ModProjectile
    {
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public bool Homing { get => Projectile.ai[1] == 1; set => Projectile.ai[1] = value ? 1 : 0; }
        public float Timer { get => Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        Vector2 targetPosition = Vector2.Zero;
        private int minFrame = 0;
        private int maxFrame = 3;

        public override void SetDefaults()
        {
            Projectile.width = (int)TextureAssets.Projectile[ProjectileID.SharknadoBolt].Size().X;
            Projectile.height = (int)(TextureAssets.Projectile[ProjectileID.SharknadoBolt].Size().Y / 3);
            Main.projFrames[Projectile.type] = 3;
            Projectile.frame = 0;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 180; // 3600
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.SharknadoBolt}";

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {
            float rotationTimer = Timer * 0.8f;
            float xVal = MathF.Sin(rotationTimer);
            float yVal = MathF.Cos(rotationTimer);

            if (Homing)
            {
                TargetingSystem();
                Projectile.velocity = Projectile.Center.DirectionTo(targetPosition) * 8;
            } else
            {
                Projectile.position += new Vector2(xVal, yVal) * 6;
            }

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.FishronWings, -Projectile.velocity.X * 0.5f,
                    -Projectile.velocity.Y * 0.5f);
            }

            ControlAnimations();

            Timer++;
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetMaxDamage(1);
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SetMaxDamage(1);
            base.ModifyHitPlayer(target, ref modifiers);
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 offset = Vector2.Zero;
            int maxSize = 9;

            if (Homing)
            {
                maxSize = 12;
                offset = new Vector2(0, -128);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, 
                Vector2.Zero, ModContent.ProjectileType<Sharknado>(), Projectile.damage, 
                3, Projectile.owner, TargetNPC, maxSize, maxSize);
        }

        public override void PostDraw(Color lightColor)
        {
            /*Asset<Texture2D> asset = TextureAssets.DukeFishron;
            Rectangle bounds = asset.Frame(1, 8, 0, Projectile.frame);
            Main.EntitySpriteDraw(
                asset.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                Color.White * eyeglowOpacity,
                Projectile.rotation,
                bounds.Size() / 2,
                1,
                Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
            );*/
            base.PostDraw(lightColor);
        }
    }
}
