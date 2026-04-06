using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GvMod.Common.Players;
using GvMod.Common.Utils;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class CthulhuServant : ModProjectile
    {
        public Vector2 targetPosition = Vector2.Zero;
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int ParentProjectile { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

        public override void SetDefaults()
        {
            Projectile.width = (int)(TextureAssets.Npc[NPCID.ServantofCthulhu].Size().X * 0.8f);
            Projectile.height = (int)(TextureAssets.Npc[NPCID.ServantofCthulhu].Size().Y * 0.4f);
            Main.projFrames[Projectile.type] = 2;
            Projectile.frame = 0;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/NPC_{NPCID.ServantofCthulhu}";


        public override void AI()
        {
            TargetingSystem();

            Vector2 newVelocity = Projectile.Center.DirectionTo(targetPosition) * 4;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, newVelocity, 0.006f);

            float recoilSpeed = 0.7f;

            float halfWidth = Projectile.width / 2f;
            float halfHeight = Projectile.height / 2f;
            Rectangle left = new Rectangle(
                    (int)(Projectile.Center.X - halfWidth) - 10, (int)(Projectile.Center.Y - halfHeight),
                    10, Projectile.height
                );
            Rectangle right = new Rectangle(
                    (int)(Projectile.Center.X + halfWidth), (int)(Projectile.Center.Y - halfHeight),
                    10, Projectile.height
                );
            Rectangle up = new Rectangle(
                    (int)(Projectile.Center.X - halfWidth), (int)(Projectile.Center.Y - halfHeight) - 10,
                    Projectile.width, 10
                );
            Rectangle down = new Rectangle(
                    (int)(Projectile.Center.X - halfWidth), (int)(Projectile.Center.Y + halfHeight),
                    Projectile.width, 10
                );

            if (Collision.SolidCollision(left.TopLeft(), left.Width, left.Height) ||
                Collision.SolidCollision(right.TopLeft(), right.Width, right.Height))
            {
                Projectile.velocity.X *= -recoilSpeed;
            }
            if (Collision.SolidCollision(up.TopLeft(), up.Width, up.Height) ||
                Collision.SolidCollision(down.TopLeft(), down.Width, down.Height))
            {
                Projectile.velocity.Y *= -recoilSpeed;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            ControlAnimations();
        }

        public void TargetingSystem()
        {
            float maxDistance = 3200;

            if (TargetNPC == -1)
            {
                // Try to find a target
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

                // If failed, try to find the parent
                if (Main.myPlayer == Projectile.owner)
                {
                    if (ParentProjectile != -1)
                    {
                        Projectile parent = Main.projectile[ParentProjectile];
                        if (parent.active && parent.ModProjectile is EyeOfCthulhu)
                        {
                            targetPosition = parent.Center;
                        } else
                        {
                            ParentProjectile = -1;
                            targetPosition = Projectile.velocity;
                        }
                    } else
                    {
                        targetPosition = Projectile.velocity;
                    }
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
                        if (ParentProjectile != -1)
                        {
                            Projectile parent = Main.projectile[ParentProjectile];
                            if (parent.active && parent.ModProjectile is EyeOfCthulhu)
                            {
                                targetPosition = parent.Center;
                            }
                            else
                            {
                                ParentProjectile = -1;
                                targetPosition = Projectile.velocity;
                            }
                        }
                        else
                        {
                            targetPosition = Projectile.velocity;
                        }
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
            if (Projectile.frameCounter >= 12)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }

            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            if (Projectile.frame < 0)
            {
                Projectile.frame = 0;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.dayTime)
            {
                modifiers.SourceDamage -= 0.25f;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
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

            if (Projectile.timeLeft <= 1)
            {
                Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);

                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    goreVel.RotatedByRandom(MathHelper.TwoPi), 6);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    goreVel.RotatedByRandom(MathHelper.TwoPi), 7);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 8;

            if (Main.dayTime)
            {
                Projectile.timeLeft -= 8;
            }

            if (Projectile.timeLeft <= 1)
            {
                Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);

                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    goreVel.RotatedByRandom(MathHelper.TwoPi), 6);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    goreVel.RotatedByRandom(MathHelper.TwoPi), 7);
            }
        }
    }
}
