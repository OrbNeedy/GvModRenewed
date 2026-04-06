using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using GvMod.Common.Players;
using GvMod.Common.Utils;
using Terraria.Audio;
using Terraria.DataStructures;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class Plantera : ModProjectile
    {
        public Vector2 targetPosition = Vector2.Zero;
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int State { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int Timer { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public Vector2[] hookPositions = [Vector2.Zero, Vector2.Zero, Vector2.Zero];
        public Vector2[] hookTargetPositions = [Vector2.Zero, Vector2.Zero, Vector2.Zero];
        private int minFrame = 0;
        private int maxFrame = 4;

        public override void SetDefaults()
        {
            Main.instance.LoadNPC(NPCID.PlanterasHook);
            Projectile.width = (int)TextureAssets.Npc[NPCID.Plantera].Size().X;
            Projectile.height = (int)(TextureAssets.Npc[NPCID.Plantera].Size().Y / 8);
            Main.projFrames[Projectile.type] = 8;
            Projectile.frame = 0;

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

        public override string Texture => $"Terraria/Images/NPC_{NPCID.Plantera}";

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < hookPositions.Length; i++)
            {
                Vector2 hookPosition = Projectile.Center + new Vector2(0, 768).RotatedByRandom(MathHelper.TwoPi);
                hookPositions[i] = hookPosition;
                hookTargetPositions[i] = FindValidTile(new Vector2(0, -1), Projectile.Center, hookPosition);
            }
            Projectile.netUpdate = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(targetPosition);
            for (int i = 0; i < hookPositions.Length; i++)
            {
                writer.WriteVector2(hookPositions[i]);
                writer.WriteVector2(hookTargetPositions[i]);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetPosition = reader.ReadVector2();
            for (int i = 0; i < hookPositions.Length; i++)
            {
                hookPositions[i] = reader.ReadVector2();
                hookTargetPositions[i] = reader.ReadVector2();
            }
        }

        public override void AI()
        {
            TargetingSystem();
            Vector2 newVelocity = Vector2.Zero;
            float lerpAmount = 0.009f;
            float timerMod = 1 - ((float)Projectile.timeLeft / 3600f);
            bool despawnAllow = false;

            if (TargetNPC != -1)
            {
                float distanceToTarget = Projectile.Center.Distance(targetPosition);
                Vector2 directionToTarget = Projectile.Center.DirectionTo(targetPosition);
                switch (State)
                {
                    case -1:
                        newVelocity = new Vector2(0, 22);
                        Projectile.rotation = directionToTarget.ToRotation()
                            + MathHelper.PiOver2;

                        if (Timer >= 300 || Projectile.Center.X >= Main.tile.Height)
                        {
                            Projectile.timeLeft = -1;
                            Projectile.active = false;
                        } else
                        {
                            Projectile.timeLeft++;
                        }
                        break;
                    case 0:
                        despawnAllow = true;
                        if (distanceToTarget > 400)
                        {
                            newVelocity = directionToTarget * 15;
                        } else
                        {
                            newVelocity = directionToTarget * -15;
                        }

                        if (Timer >= 90 - (timerMod * 30) &&
                            Collision.CanHitLine(Projectile.Center, 8, 8, targetPosition, 8, 8))
                        {
                            int projectile = ModContent.ProjectileType<PlanteraSeed>();
                            int poison = 0;
                            float seed = Main._rand.NextFloat();
                            if (seed < 0.6)
                            {
                                poison = 1;
                                if (seed < 0.1f)
                                {
                                    projectile = ModContent.ProjectileType<ThornBall>();
                                }
                            }

                            if (Main.myPlayer == Projectile.owner)
                            {
                                SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                    directionToTarget * 8, projectile,
                                    (int)(Projectile.damage * 0.6f), 1.5f, Projectile.owner, TargetNPC, 
                                    poison);
                            }

                            Timer = 0;
                        }

                        if (Projectile.timeLeft <= 1800)
                        {
                            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
                            Vector2 additionalVel = Projectile.velocity * 0.9f;

                            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 378);
                            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 379);
                            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 380);

                            minFrame = 4;
                            maxFrame = 8;

                            if (Main.myPlayer == Projectile.owner)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                        Vector2.Zero, ModContent.ProjectileType<PlanteraTentacle>(),
                                        Projectile.damage, 2.5f, Projectile.owner, Projectile.whoAmI,
                                        Main._rand.NextFloat(-2, 2), Main._rand.NextFloat(0, 10)
                                        );
                                }
                            }
                            
                            Timer = 0;
                            State = 1;
                        }
                        break;
                    case 1:
                        despawnAllow = true;
                        newVelocity = directionToTarget * 16;
                        lerpAmount = 0.01f;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            if (Timer % 10 == 0 && Main._rand.NextBool(12))
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                    Vector2.Zero, ModContent.ProjectileType<PlanteraTentacle>(),
                                    Projectile.damage, 2.5f, Projectile.owner, Projectile.whoAmI,
                                    Main._rand.NextFloat(-2, 2), Main._rand.NextFloat(0, 10)
                                    );
                            }

                            // Second phase starts at half it's life, so timerMod's value is assumed to be 0.5
                            if (Timer >= 390 - (timerMod * 180))
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                    new Vector2(0, -12).RotatedByRandom(MathHelper.Pi / 3f),
                                    ModContent.ProjectileType<Spore>(), (int)(Projectile.damage),
                                    1f, Projectile.owner);
                                Timer = 0;
                            }
                        }
                        break;
                }
                Projectile.rotation = directionToTarget.ToRotation() + MathHelper.PiOver2;
            } else
            {
                if (State == -1)
                {
                    newVelocity = new Vector2(0, 22);
                    Projectile.rotation = Projectile.Center.DirectionTo(targetPosition).ToRotation()
                        + MathHelper.PiOver2;

                    if (Timer >= 300 || Projectile.Center.X >= Main.tile.Height)
                    {
                        Projectile.timeLeft = -1;
                        Projectile.active = false;
                    }
                    else
                    {
                        Projectile.timeLeft++;
                    }
                } else
                {
                    despawnAllow = true;
                    Player owner = Main.player[Projectile.owner];
                    targetPosition = owner.Center;

                    Vector2 offset = new Vector2(-448 * owner.direction, -288);
                    newVelocity = Projectile.Center.DirectionTo(
                        targetPosition + offset
                        ) * 15;

                    Projectile.rotation = Projectile.Center.DirectionTo(targetPosition).ToRotation()
                        + MathHelper.PiOver2;
                }
            }

            if (despawnAllow && Projectile.timeLeft <= 3)
            {
                State = -1;
                Projectile.timeLeft++;
            }

            // Check movement availability, move if needed
            for (int i = 0; i < hookPositions.Length; i++)
            {
                float distanceToTarget = hookPositions[i].Distance(hookTargetPositions[i]);
                if (hookPositions[i] != hookTargetPositions[i])
                {
                    hookPositions[i] += hookPositions[i].DirectionTo(hookTargetPositions[i]) * 6;
                }
                if (distanceToTarget > 6)
                {
                    continue;
                }

                float distance = hookPositions[i].Distance(targetPosition);
                Vector2 direction = Projectile.Center.DirectionTo(targetPosition);
                //Main.NewText($"Hook {i} distance: {distance}");
                if (distance >= Main._rand.NextFloat(1280, 1601))
                {
                    Vector2 oldHookTargetPosition = hookTargetPositions[i];
                    hookTargetPositions[i] = FindValidTile(direction, Projectile.Center, hookTargetPositions[i]);
                    if (oldHookTargetPosition != hookTargetPositions[i])
                    {
                        Projectile.netUpdate = true;
                    }
                }
                //Main.NewText($"Post hook {i} position: {hookPositions[i]}");
                //Main.NewText($"Post hook {i} target: {hookTargetPositions[i]}");
            }

            Vector2 centeredPosition = Vector2.Lerp(hookPositions[0], hookPositions[1], 0.5f);
            centeredPosition = Vector2.Lerp(centeredPosition, hookPositions[2], 1f / 3f);

            float vineDistance = Projectile.Center.Distance(centeredPosition);
            Vector2 vinePull = Projectile.Center.DirectionTo(centeredPosition) * (20 * (vineDistance / 800));
            if (vineDistance <= 0.001f)
            {
                vinePull = Vector2.Zero;
            }

            newVelocity += vinePull;

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, newVelocity, lerpAmount);

            ControlAnimations();
            Timer++;
        }

        public Vector2 FindValidTile(Vector2 direction, Vector2 searchOrigin, Vector2 originalPosition)
        {
            Vector2 returnPosition = originalPosition;
            for (int d = 0; d < 10; d++)
            {
                Vector2 rotatedDirection = direction.RotatedByRandom(MathHelper.Pi);
                //Main.NewText($"Check direction: {d * 0.08f}");
                bool breakRotation = false;
                for (int k = 800; k >= 0; k -= 16)
                {
                    Vector2 tileAsWorld = searchOrigin + (direction * k);
                    Point16 tilePos = tileAsWorld.ToTileCoordinates16();
                    Tile tile = Framing.GetTileSafely(tilePos.X, tilePos.Y);
                    if (tile.HasTile || tile.WallType != WallID.None)
                    {
                        //Main.NewText($"Selected target: {tileAsWorld}");
                        //Main.NewText($"Tile: {tilePos}");
                        returnPosition = tileAsWorld;
                        breakRotation = true;
                        break;
                    }
                }
                if (breakRotation) break;
            }
            return returnPosition;
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (State == 1)
            {
                modifiers.SourceDamage += 0.5f;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (State == 1)
            {
                modifiers.SourceDamage += 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft -= 4;
            target.AddBuff(BuffID.Poisoned, 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 8;
            target.AddBuff(BuffID.Poisoned, 180);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
            Vector2 additionalVel = Projectile.velocity * 0.9f;

            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 381);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 382);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 383);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 384);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 385);
            Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 386);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDrawExtras()
        {
            Asset<Texture2D> asset = TextureAssets.Npc[NPCID.PlanterasHook];
            Asset<Texture2D> vine = TextureAssets.Chain26;
            Rectangle vineBounds = vine.Frame();
            for (int i = 0; i < hookPositions.Length; i++)
            {
                Vector2 hookPosition = hookPositions[i];
                Vector2 hookTarget = hookTargetPositions[i];
                float distanceToTarget = hookPosition.Distance(hookTarget);

                int frame = (int)MathHelper.Clamp(((distanceToTarget - 32) / 48), 0, 2);

                if (distanceToTarget <= 6)
                {
                    hookPosition = hookTarget;
                    frame = 0;
                }

                Rectangle bounds = asset.Frame(1, 4, 0, frame);

                float distance = hookPosition.Distance(Projectile.Center);
                Vector2 vineDirection = hookPosition.DirectionTo(Projectile.Center);
                Vector2 initialDistance = vineDirection * vine.Height() / 2f;

                for (int k = 0; k < distance; k += vine.Height())
                {
                    Vector2 offset = vineDirection * k;
                    Vector2 vinePos = hookPosition + initialDistance + offset;
                    Main.EntitySpriteDraw(
                        vine.Value,
                        vinePos - Main.screenPosition,
                        vineBounds,
                        Lighting.GetColor(vinePos.ToTileCoordinates()),
                        Projectile.Center.AngleTo(hookPosition) + MathHelper.PiOver2,
                        vineBounds.Size() / 2f,
                        1,
                        SpriteEffects.None
                    );
                }

                Main.EntitySpriteDraw(
                    asset.Value,
                    hookPosition - Main.screenPosition,
                    bounds,
                    Lighting.GetColor(hookPosition.ToTileCoordinates()),
                    Projectile.Center.AngleTo(hookPosition) + MathHelper.PiOver2,
                    bounds.Size() / 2,
                    1,
                    SpriteEffects.None
                );
            }

            return base.PreDrawExtras();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            bool colliding = false;
            Vector2 hookSize = new Vector2(24);

            for (int i = 0; i < hookPositions.Length; i++)
            {
                if (Collision.CheckAABBvAABBCollision(hookPositions[i] - (hookSize / 2f), 
                    hookSize, targetHitbox.TopLeft(), targetHitbox.Size()))
                {
                    colliding = true;
                    break;
                }
            }
            if (colliding) return true;

            return base.Colliding(projHitbox, targetHitbox);
        }
    }
}
