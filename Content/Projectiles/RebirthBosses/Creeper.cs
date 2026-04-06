using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class Creeper : ModProjectile
    {
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public float VariationValue { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int ParentProjectile { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int Timer { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public bool attacking = false;
        Vector2 targetPosition = Vector2.Zero;

        public override void SetDefaults()
        {
            Projectile.width = (int)(TextureAssets.Npc[NPCID.Creeper].Size().X);
            Projectile.height = (int)(TextureAssets.Npc[NPCID.Creeper].Size().Y);
            //Main.projFrames[Projectile.type] = 16;
            //Projectile.frame = 0;
            //Projectile.light = 0.5f;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 1800; // 3600
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/NPC_{NPCID.Creeper}";

        public override void OnSpawn(IEntitySource source)
        {
            //Mod.Logger.Debug("Reached Creeper spawn");
        }

        public override void AI()
        {
            if (ParentProjectile != -1)
            {
                Projectile proj = Main.projectile[ParentProjectile];
                if (proj.active && proj.owner == Projectile.owner && 
                    proj.ModProjectile is BrainOfCthulhu)
                {
                    if (TargetNPC == -1 || !attacking)
                    {
                        targetPosition = proj.Center;
                    } else
                    {
                        targetPosition = Main.npc[TargetNPC].Center;
                    }
                } else
                {
                    Projectile.active = false;
                    Projectile.timeLeft = -1;
                    return;
                }
            } else
            {
                Projectile.active = false;
                Projectile.timeLeft = -1;
                return;
            }

            int detractedTime = attacking ? 60 : 0;
            if (Timer >= 180 - detractedTime + (VariationValue * 60))
            {
                attacking = !attacking;
                Timer = 0;
            }

            // Invalid NPC target or not attacking
            if (TargetNPC == -1 || !attacking || Main.projectile[ParentProjectile].ai[1] == -1)
            {
                Move();
            } else
            {
                HomeInEnemy();
            }

            Timer++;
        }

        public void Move()
        {
            float velX = 0;
            float velY = 0;
            if (Projectile.Center.X > targetPosition.X)
            {
                velX -= 0.125f;
            }
            if (Projectile.Center.X < targetPosition.X)
            {
                velX += 0.125f;
            }
            if (Projectile.Center.Y > targetPosition.Y)
            {
                velY -= 0.125f;
            }
            if (Projectile.Center.Y < targetPosition.Y)
            {
                velY += 0.125f;
            }

            Projectile.velocity += new Vector2(velX, velY);
            if (Projectile.velocity.Length() > 10)
            {
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 10;
            }
        }

        public void HomeInEnemy()
        {
            if (Projectile.Center == targetPosition) return;

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, 
                Projectile.Center.DirectionTo(targetPosition) * 10, 0.06f);
        }

        public void Phase1AI()
        {
            if (Projectile.Center == targetPosition) return;

            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 3.5f, 0.1f);

            if (Timer >= 480)
            {
                Projectile.Opacity -= 1f / 45f;
                if (Projectile.Opacity <= 0)
                {
                    Projectile.Center = targetPosition + new Vector2(640, 0).RotatedByRandom(MathHelper.TwoPi);
                    Timer = 0;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.Opacity += 1f / 45f;
                Timer++;
            }
        }

        public void Phase2AI()
        {

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft -= 2;
            int buffType = Main._rand.Next([BuffID.Poisoned, BuffID.Bleeding,
                BuffID.Confused, BuffID.BrokenArmor]);
            target.AddBuff(buffType, 120);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 4;
            int buffType = Main._rand.Next([BuffID.Poisoned, BuffID.Darkness,
                BuffID.Cursed, BuffID.Bleeding, BuffID.Confused, BuffID.Slow,
                BuffID.Weak, BuffID.Silenced, BuffID.BrokenArmor]);
            target.AddBuff(buffType, 90);
        }

        public override bool PreDrawExtras()
        {
            return base.PreDrawExtras();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
            Vector2 additionalVel = Projectile.velocity * 0.9f;

            if (ParentProjectile != -1)
            {
                Projectile parent = Main.projectile[ParentProjectile];
                if (parent.active && parent.owner == Projectile.owner && 
                    parent.ModProjectile is BrainOfCthulhu brain)
                {

                }
            }

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
