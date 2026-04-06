using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using GvMod.Common.Players;
using GvMod.Common.Utils;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace GvMod.Content.Projectiles.RebirthBosses
{
    class EaterofWorlds : ModProjectile
    {
        public int TargetNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public int NextSegment { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int PreviousSegment { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = value; }
        public int Timer { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public int Life { get => (int)Projectile.localAI[1]; set => Projectile.localAI[1] = value; }
        public int State { get => (int)Projectile.localAI[2]; set => Projectile.localAI[2] = value; }
        Vector2 targetPosition = Vector2.Zero;

        public override void SetDefaults()
        {
            Projectile.width = (int)(TextureAssets.Npc[NPCID.EaterofWorldsHead].Size().X);
            Projectile.height = (int)(TextureAssets.Npc[NPCID.EaterofWorldsHead].Size().Y);

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = false;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 5400; // 3600
            Projectile.netImportant = true;
        }

        public override string Texture => $"Terraria/Images/NPC_{NPCID.EaterofWorldsHead}";

        public override void OnSpawn(IEntitySource source)
        {
            State = CheckAdjacentSegments();
            Life = 50;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int oldestSegment = Projectile.whoAmI;
                if (NextSegment == -1 && PreviousSegment == -1)
                {
                    for (int i = 0; i < 34; i++)
                    {
                        int projIndex = Projectile.NewProjectile(
                            source, Projectile.Center, Vector2.Zero, Type,
                            Projectile.damage, Projectile.knockBack, Projectile.owner,
                            TargetNPC, oldestSegment, -1
                            );

                        if (oldestSegment != -1)
                        {
                            Main.projectile[oldestSegment].ai[2] = projIndex;
                            Main.projectile[oldestSegment].netUpdate = true;
                        }

                        oldestSegment = projIndex;
                    }
                }
            }

            Projectile.netUpdate = true;
            //Main.NewText("State: " + State);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
        }

        public override void AI()
        {
            int newState = CheckAdjacentSegments();
            if (Life <= 0 || newState < 0)
            {
                Projectile.active = false;
                Projectile.timeLeft = -1;
                Projectile.netUpdate = true;
                return;
            }

            State = newState;

            switch (State)
            {
                case 0: // Head
                    TargetingSystem();

                    HeadAI();
                    break;
                case 1: // Body
                case 2: // Tail
                    SetFollowerTarget();

                    FollowerAI();
                    break;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void HeadAI()
        {
            Point16 pos = Projectile.Center.ToTileCoordinates16();
            Tile tile = Framing.GetTileSafely(pos);

            if (Projectile.timeLeft <= 3)
            {
                Projectile.timeLeft++;
                Timer++;

                Projectile.velocity.Y += 0.08f;

                if (tile.HasTile)
                {
                    Projectile.velocity.X = float.Lerp(Projectile.velocity.X, 0, 0.0099f);
                    Projectile.velocity.Y += 0.16f;
                }
                else
                {
                    Projectile.velocity.X = float.Lerp(Projectile.velocity.X, 0, 0.008f);
                }

                if (Timer >= 600 || Projectile.Center.Y <= Main.maxTilesY)
                {
                    Projectile.timeLeft = -1;
                    Projectile.active = false;

                    if (PreviousSegment != -1)
                    {
                        Main.projectile[PreviousSegment].timeLeft = -1;
                        Main.projectile[PreviousSegment].active = false;
                    }
                }
                return;
            }

            if (Projectile.Center == targetPosition)
            {
                return;
            }

            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);
            Vector2 newVelocity = direction * 12;

            if (tile.HasTile)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, newVelocity, 0.0085f);
            } else
            {
                Projectile.velocity.X = float.Lerp(Projectile.velocity.X, newVelocity.X * 0.5f, 0.008f);
                Projectile.velocity.Y += 0.04f;
            }
        }

        public void FollowerAI()
        {
            if (Projectile.timeLeft <= 3)
            {
                Projectile.timeLeft = 3;
            }

            if (Projectile.Center == targetPosition)
            {
                return;
            }

            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPosition, 0.01f);

            Vector2 direction = Projectile.Center.DirectionTo(targetPosition);
            float distance = Projectile.Center.Distance(targetPosition);
            float maxDistance = Projectile.height * 0.5f;

            if (distance > maxDistance || distance < maxDistance)
            {
                Projectile.Center = targetPosition +
                    (targetPosition.DirectionTo(Projectile.Center) * maxDistance);
            }

            Projectile.velocity = direction;
        }

        public override bool ShouldUpdatePosition()
        {
            return State <= 0;
        }

        public int CheckAdjacentSegments()
        {
            int type = 1;

            bool validNext = CheckSegmentValidity(NextSegment);
            bool validPrevious = CheckSegmentValidity(PreviousSegment);

            // There is a head or body next to this segment
            if (validNext)
            {
                // There is no segments after this one
                if (!validPrevious)
                {
                    type = 2; // It's a tail
                }
                return type;
            }

            // There is no head, but more segments before this one
            if (validPrevious)
            {
                type = 0;
            } else
            {
                // There are no segments connected to this one
                type = -2;
            }
            return type;
        }

        public bool CheckSegmentValidity(int segment)
        {
            if (segment != -1)
            {
                Projectile nextProj = Main.projectile[segment];
                if (nextProj.ModProjectile is EaterofWorlds head && nextProj.active &&
                    nextProj.owner == Projectile.owner)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetFollowerTarget()
        {
            if (CheckSegmentValidity(NextSegment))
            {
                targetPosition = Main.projectile[NextSegment].Center;
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
            if (State == 0)
            {
                modifiers.SourceDamage += 0.5f;
            }

            if (State == 2)
            {
                modifiers.SourceDamage -= 0.2f;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (State == 0)
            {
                modifiers.SourceDamage += 0.5f;
            }

            if (State == 2)
            {
                modifiers.SourceDamage -= 0.2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft -= 1;
            Life--;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.timeLeft -= 2;
            Life--;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            Vector2 goreVel = new Vector2(Main._rand.NextFloat() * 2, 0);
            Vector2 additionalVel = Projectile.velocity * 0.9f;

            switch (State)
            {
                case 0:
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                        goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 24);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                        goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 25);
                    break;
                case 2:
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                        goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 28);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                        goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 29);
                    break;
                default:
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                        goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 26);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                        goreVel.RotatedByRandom(MathHelper.TwoPi) + additionalVel, 27);
                    break;
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

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = TextureAssets.Npc[NPCID.EaterofWorldsBody];

            switch (State)
            {
                case 0: // Head
                    texture = TextureAssets.Npc[NPCID.EaterofWorldsHead];
                    break;
                case 2: // Tail
                    texture = TextureAssets.Npc[NPCID.EaterofWorldsTail];
                    break;
            }

            Main.EntitySpriteDraw(
                texture.Value,
                Projectile.Center - Main.screenPosition,
                texture.Frame(), 
                lightColor, 
                Projectile.rotation, 
                texture.Size() / 2f, 
                1, 
                SpriteEffects.None
                );

            return false;
        }
    }
}
