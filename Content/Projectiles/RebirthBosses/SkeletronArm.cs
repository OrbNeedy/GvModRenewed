using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.IO;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class SkeletronArm : ModProjectile
    {
        public Vector2 targetPosition = Vector2.Zero;
        public int ParentProjectile { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int State { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int SlashCount { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int Timer { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public float jointRotation = 0;
        public Vector2 jointPosition = Vector2.Zero;

        public override void SetDefaults()
        {
            Projectile.width = (int)TextureAssets.Npc[NPCID.SkeletronHand].Size().X;
            Projectile.height = (int)(TextureAssets.Npc[NPCID.SkeletronHand].Size().Y);
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

        public override string Texture => $"Terraria/Images/NPC_{NPCID.SkeletronHand}";

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
            Vector2 newVelocity = Vector2.Zero;
            float lerpAmount = 0.025f;

            float newRotation = 0f;
            float rotationLerp = 0.04f;

            if (Main.myPlayer == Projectile.owner && Main.projectile[ParentProjectile].active 
                && Main.projectile[ParentProjectile].ModProjectile is Skeletron head)
            {
                Projectile parent = Main.projectile[ParentProjectile];
                // Assume the head's TargetNPC to always be right
                //Main.NewText("State: " + State, new Color(255, 255, 0));
                if (head.TargetNPC != -1)
                {
                    Vector2 targetPosition = head.targetPosition;
                    // Attack target
                    //Main.NewText("Target found ", new Color(0, 255, 0));
                    switch (State)
                    {
                        // Idle (Not slashing)
                        case -1:
                        case 1:
                            Vector2 idlePositionOffset = new Vector2(128 * State, 192);
                            newRotation = 0 + (Projectile.velocity.X * 0.075f);

                            if (head.State == 1 || head.State == 2)
                            {
                                idlePositionOffset.Y *= -1;
                                newRotation = -MathHelper.Pi * State;
                            }

                            newVelocity = Projectile.DirectionTo(parent.Center + idlePositionOffset) * 10;

                            if (Timer >= 120 && head.State != 1)
                            {
                                //Main.NewText($"Switching state from {State} to {State * 2}", new Color(255, 255, 0));
                                State *= 2;
                                Timer = 0;
                            }
                            break;
                        // Slashing
                        case -2:
                        case 2:
                            if (Timer <= 90)
                            {
                                if (Timer == 90)
                                {
                                    newVelocity = Projectile.Center.DirectionTo(targetPosition) * 48;
                                    lerpAmount = 1f;
                                    SlashCount++;
                                } else
                                {
                                    int slashReversal = SlashCount % 2 == 0 ? 1 : -1;
                                    Vector2 slashPreparePosition = new Vector2(
                                        -64 + (80 * State * slashReversal),
                                        -64 * State * slashReversal
                                        );

                                    newVelocity = Projectile.Center.DirectionTo(parent.Center + slashPreparePosition) * 8;
                                    lerpAmount = 0.1f;
                                    newRotation = Projectile.Center.AngleTo(targetPosition) * (State / 2);
                                    rotationLerp = 0.1f;
                                }
                            } else
                            {
                                lerpAmount = 0.08f;
                            }

                            if (Timer >= 120)
                            {
                                State /= 2;
                                Timer = 0;
                            }
                            break;
                    }

                    ControlJoints(parent.Center);
                } else
                {
                    // Follow the head
                    Vector2 idlePositionOffset = new Vector2(128 * State, 192);

                    newVelocity = Projectile.DirectionTo(parent.Center + idlePositionOffset) * 8;
                    newRotation = 0 + (Projectile.velocity.X * 0.075f);
                }
            } else
            {
                // If parent projectile does not exists, destroy self
                Projectile.timeLeft = 0;
                return;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, newVelocity, lerpAmount);
            Projectile.rotation = float.Lerp(Projectile.rotation, newRotation, rotationLerp);

            Timer++;
        }

        public void ControlJoints(Vector2 headPosition)
        {
            Asset<Texture2D> handBones = TextureAssets.BoneArm;

            float jointDistance = jointPosition.Distance(Projectile.Center);

            jointPosition = Vector2.Lerp(jointPosition, headPosition + new Vector2(State > 0 ? 46 : -46, 0), 0.01f);

            // TODO: Constraint 1: Depending on the side, the angle to the hand will never be more than a certain number

            // Constraint 2: Joint position will never be greater or less than the height of the bone sprite
            if (jointDistance > handBones.Height() || jointDistance < handBones.Height())
            {
                jointPosition = Projectile.Center +
                    (Projectile.Center.DirectionTo(jointPosition) * handBones.Height());
            }

            Vector2 dustVel = new Vector2(0, 1 + (Main._rand.NextFloat() * 2)).
                RotatedBy(jointPosition.AngleTo(headPosition)).
                RotatedByRandom(MathHelper.PiOver4 / 2);
            Vector2 dustPosition = jointPosition + (jointPosition.DirectionTo(headPosition) * 
                handBones.Height());
            Dust.NewDust(dustPosition, 0, 0, DustID.Blood, dustVel.X, dustVel.Y, Scale: 2);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft -= 10;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 20;
        }

        public override void OnKill(int timeLeft)
        {
            // Spawn gore
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);

            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) +
                (Projectile.velocity * 0.9f), 56);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) +
                (Projectile.velocity * 0.9f), 57);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.spriteDirection = State < 0 ? 1 : -1;
            Asset<Texture2D> hand = TextureAssets.Npc[NPCID.SkeletronHand];
            Main.EntitySpriteDraw(
                hand.Value, 
                Projectile.Center - Main.screenPosition, 
                hand.Frame(), 
                lightColor, 
                Projectile.rotation, 
                new Vector2(hand.Width() / 2, 0), 
                Projectile.scale,
                State < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally
                );

            return false;
        }

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> handBones = TextureAssets.BoneArm;

            Vector2 headPosition = Vector2.Zero;
            float jointDistance = jointPosition.Distance(Projectile.Center);
            float jointAngle = jointPosition.AngleTo(Projectile.Center) - jointPosition.AngleTo(headPosition);
            int side = State < 0 ? -1 : 1;

            // Assume position of 0, 0 if no head is present
            if (ParentProjectile != -1)
            {
                headPosition = Main.projectile[ParentProjectile].Center;
            }

            // Constraint 2: Joint position will never be greater or less than the height of the bone sprite
            if (jointDistance > handBones.Height() || jointDistance < handBones.Height())
            {
                jointPosition = Projectile.Center + 
                    (Projectile.Center.DirectionTo(jointPosition) * handBones.Height());
            }

            Main.EntitySpriteDraw(
                handBones.Value,
                Projectile.Center - Main.screenPosition,
                handBones.Frame(),
                Lighting.GetColor(Projectile.Center.ToTileCoordinates()),
                Projectile.Center.AngleTo(jointPosition) + MathHelper.PiOver2, 
                new Vector2(handBones.Width() / 2, handBones.Height()), 
                1, 
                State > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally
                );

            Main.EntitySpriteDraw(
                handBones.Value,
                jointPosition - Main.screenPosition,
                handBones.Frame(),
                Lighting.GetColor(jointPosition.ToTileCoordinates()),
                jointPosition.AngleTo(headPosition) + MathHelper.PiOver2,
                new Vector2(handBones.Width() / 2, handBones.Height()),
                1,
                State > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally
                );

            return base.PreDrawExtras();
        }
    }
}
