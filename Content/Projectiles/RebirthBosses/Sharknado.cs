using GvMod.Common.Players;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class Sharknado : ModProjectile
    {
        public Vector2 targetPosition = Vector2.Zero;
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int AdditionalSharknados { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int MaxSharknados { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int Timer { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        private int minFrame = 0;
        private int maxFrame = 6;

        public override void SetDefaults()
        {
            Projectile.width = (int)TextureAssets.Projectile[ProjectileID.Sharknado].Size().X;
            Projectile.height = (int)(TextureAssets.Projectile[ProjectileID.Sharknado].Size().Y / 6);
            Main.projFrames[Projectile.type] = 6;
            Projectile.frame = 0;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 360; // 3600
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Sharknado}";

        public override void OnSpawn(IEntitySource source)
        {
            float sizeDiff = ((float)AdditionalSharknados / (float)MaxSharknados) * 0.6f;

            Projectile.scale = 1.2f - sizeDiff;
        }

        public override void AI()
        {
            TargetingSystem();

            Projectile.velocity.X = MathF.Sin(Timer * 0.16f) * 6f;

            if (Timer >= MathHelper.Pi && AdditionalSharknados > 0)
            {
                float sizeDiff = ((float)AdditionalSharknados / (float)MaxSharknados) * 0.5f;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + 
                    new Vector2(0, -Projectile.height * Projectile.scale), Vector2.Zero, Type, Projectile.damage, 
                    Projectile.knockBack, Projectile.owner, TargetNPC, AdditionalSharknados - 1, 
                    MaxSharknados);
                AdditionalSharknados = -AdditionalSharknados;
                Timer = 0;
            }

            //Main.NewText($"Additional {AdditionalSharknados}");
            //Main.NewText($"Max {MaxSharknados}");

            if (Timer >= 60 && Timer % 30 == 0 && AdditionalSharknados == -MaxSharknados)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                    ModContent.ProjectileType<Sharkron>(), (int)(Projectile.damage * 0.8f), 1, 
                    Projectile.owner, ai2: MaxSharknados);
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

        public override void OnKill(int timeLeft)
        {
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
        }
    }
}
