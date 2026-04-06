using GvMod.Common.Players;
using GvMod.Common.Utils;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GvMod.Content.Items.Corpses;

namespace GvMod.Content.Projectiles.RebirthSummons
{
    class ZombieSlime : ModProjectile
    {
        public int MaxVisualFrame = 2;
        public int MinVisualFrame = 0;
        public int MaxFrameCounter = 14;
        int colorTimer = 90;
        int oldColorIndex = 0;
        int colorIndex = 0;
        Color color = Color.White;

        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int JumpCounter { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public int JumpTimer { get => (int)Projectile.localAI[1]; set => Projectile.localAI[1] = value; }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 24;
            Projectile.scale = 1f;
            MaxVisualFrame = Main.projFrames[Projectile.type] = 2;
            Projectile.frame = 0;

            Projectile.DamageType = ModContent.GetInstance<SeptimaSummonHybrid>();
            Projectile.damage = 6;
            Projectile.knockBack = 1;
            Projectile.penetrate = -1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 14;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 480;
            Projectile.netImportant = true;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            float drag = 0.02f;

            TargetNPC = ZombieBat.TargetingSystem(Projectile, TargetNPC, 640);

            ControlAnimations();

            bool hasTarget = TargetNPC != -1;
            Vector2 targetPosition = Projectile.Center;
            bool groundCollision = ZombieScorpion.GroundCollision(Projectile);

            if (hasTarget)
            {
                targetPosition = Main.npc[TargetNPC].Center;

                if (groundCollision && JumpTimer >= 20)
                {
                    Vector2 direction = Projectile.Center.DirectionTo(targetPosition);
                    if (Projectile.Center == targetPosition)
                    {
                        direction = Vector2.Zero;
                    }

                    if (direction.X == 0)
                    {
                        direction.X = 1 - (2 * Main._rand.Next(0, 2));
                        Projectile.netUpdate = true;
                    }

                    int jumpDirection = (direction.X > 0 ? 1 : -1);

                    JumpTimer = 0;
                    if (JumpCounter < 3)
                    {
                        Projectile.velocity.Y = -6f;
                        Projectile.velocity.X = 6f * jumpDirection;
                    } else
                    {
                        Projectile.velocity.Y = -12f;
                        Projectile.velocity.X = 1.5f * jumpDirection;
                        JumpCounter = 0;
                    }
                    Projectile.position.Y -= 4;
                    JumpCounter++;
                }
            }
            else
            {
                targetPosition = Projectile.Center + Projectile.velocity;

                if (groundCollision && JumpTimer >= 45)
                {
                    Vector2 direction = Projectile.Center.DirectionTo(targetPosition);
                    if (Projectile.Center == targetPosition)
                    {
                        direction = Vector2.Zero;
                    }

                    if (direction.X == 0)
                    {
                        direction.X = 1 - (2 * Main._rand.Next(0, 2));
                        Projectile.netUpdate = true;
                    }

                    int jumpDirection = (direction.X > 0 ? 1 : -1);

                    JumpTimer = 0;
                    Projectile.velocity.Y = -4f;
                    Projectile.velocity.X = 6f * jumpDirection;
                    Projectile.position.Y -= 3;
                }
                JumpCounter = 0;
            }

            if (groundCollision)
            {
                JumpTimer++;
            }

            Vector2 newVelocity = ZombieScorpion.GroundZombieMovement(Projectile, drag, groundCollision);

            Projectile.velocity = newVelocity;

            MinVisualFrame = 0;
            MaxVisualFrame = 2;

            ColorUpdate();
        }

        private void ColorUpdate()
        {
            color = Color.Lerp(SlimeCorpse.slimeCorpseColorList[oldColorIndex],
                SlimeCorpse.slimeCorpseColorList[colorIndex], colorTimer / 90f);

            if (colorTimer >= 60)
            {
                oldColorIndex = colorIndex;
                colorIndex = Main._rand.Next(0, SlimeCorpse.slimeCorpseColorList.Length);
                colorTimer = 0;
            }
            else
            {
                colorTimer++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
                Tag tag = adept.TaggedNPCs.GetTag(target.whoAmI);
                if (tag.targetIndex == target.whoAmI)
                {
                    adept.TryTriggerTagLifesteal(damageDone);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
        }

        public void ControlAnimations()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= MaxFrameCounter)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }

            if (Projectile.frame >= MaxVisualFrame)
            {
                Projectile.frame = MinVisualFrame;
            }

            if (Projectile.frame < MinVisualFrame)
            {
                Projectile.frame = MinVisualFrame;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Point tileCoords = Projectile.Center.ToTileCoordinates();
            Color finalColor = Lighting.GetColor(tileCoords, color);
            lightColor = finalColor * 0.6f;
            return base.PreDraw(ref lightColor);
        }
    }
}
