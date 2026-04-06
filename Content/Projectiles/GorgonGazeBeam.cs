using GvMod.Common.GlobalNPCs;
using GvMod.Common.Players;
using GvMod.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class GorgonGazeBeam : ModProjectile
    {
        public static Asset<Texture2D> gazeBeam;

        public float ExtraPotency { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public bool IgnoreResistance { get => Projectile.ai[1] == 1; set => Projectile.ai[1] = value ? 1 : 0; }
        public bool IgnoreCooldown { get => Projectile.ai[2] == 1; set => Projectile.ai[2] = value ? 1 : 0; }

        int frame = 0;
        int frameTimer = 0;
        bool redden = false;
        public const int maxGorgonGazeBeamFrames = 8;
        public const float gazeLength = 1600;
        public const int gorgonGazeBeamDuration = 180;
        int gazeSoundTimer = 0;
        SlotId soundID;

        public override void SetStaticDefaults()
        {
            gazeBeam = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/GorgonGazeBeam");
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16);
            Projectile.light = 1f;
            Projectile.scale = 1f;

            Projectile.DamageType = ModContent.GetInstance<MainAttackDamage>();
            Projectile.damage = 0;
            Projectile.knockBack = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = gorgonGazeBeamDuration;
            Projectile.ownerHitCheck = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GorgonGaze") with
            {
                PitchVariance = 0.1f
            }, Projectile.Center, StopSound);
        }

        public bool StopSound(ActiveSound sound)
        {
            gazeSoundTimer++;
            return Projectile.active && Projectile.ModProjectile is GorgonGazeBeam;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.velocity = Main.LocalPlayer.DirectionTo(Main.MouseWorld);

                Projectile.Center = Main.LocalPlayer.Center + (Projectile.velocity * 5) + 
                    new Vector2(0, -14);
                Projectile.netUpdate = true;
            }

            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            Main.player[Projectile.owner].itemTime = 2;
            Main.player[Projectile.owner].itemAnimation = 0;
            Main.player[Projectile.owner].direction = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();

            ActiveSound soundInstance;
            SoundEngine.TryGetActiveSound(soundID, out soundInstance);

            if (soundInstance == null)
            {
                soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GorgonGaze") with
                {
                    PitchVariance = 0.15f,
                    Volume = 0.25f
                }, Projectile.Center, StopSound);
            }
            else
            {
                if (!soundInstance.IsPlaying)
                {
                    soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GorgonGaze") with
                    {
                        PitchVariance = 0.15f,
                        Volume = 0.25f
                    }, Projectile.Center, StopSound);
                }
            }

            Animation();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void Animation()
        {
            int maxFrames = maxGorgonGazeBeamFrames - 2;
            int maxTime = frame < maxFrames? 2 : 8;
            if (Projectile.timeLeft <= maxTime * 2)
            {
                maxFrames = maxGorgonGazeBeamFrames;
            }

            if (Projectile.timeLeft == maxTime * 2)
            {
                frame = maxFrames - 2;
            }

            //Main.NewText("Max Frames: ");

            if (frameTimer++ > maxTime)
            {
                frame++;
                frameTimer = 0;
                redden = !redden;
                if (frame >= maxFrames)
                {
                    frame = 3;
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            // Only hit entities facing towards it
            if (target.direction == Projectile.direction || 
                target.GetGlobalNPC<DebuffNPC>().soulPetrified ||
                (target.GetGlobalNPC<DebuffNPC>().soulPetrificationImmunity > 0 && 
                !IgnoreCooldown))
            {
                return false;
            }
            float lifePercent = (float)target.life / (float)target.lifeMax;

            if (target.boss && lifePercent > 0.5f && !IgnoreResistance)
            {
                return false;
            }

            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            PetrifyNPC(target, 1f + ExtraPotency, IgnoreResistance, IgnoreCooldown);
        }

        /// <summary>
        /// Attempts to petrify the target, following the rules and special conditions.
        /// </summary>
        /// <param name="target"></param>
        public static void PetrifyNPC(NPC target, float potency = 1, 
            bool ignoreBossResistance = false, bool ignoreCooldown = false)
        {
            if (target.type == NPCID.Gnome)
            {
                target.life = 0;
                target.active = false;

                Point16 pos = target.Center.ToTileCoordinates16();
                Tile tile = Framing.GetTileSafely(pos);
                bool success = false;
                if (!tile.HasTile)
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, TileID.GardenGnome);
                    success = Main.tile[pos.X, pos.Y].TileType == TileID.GardenGnome;
                }

                if (!success)
                {
                    Item.NewItem(target.GetSource_Death(), target.Hitbox, ItemID.GardenGnome);
                }

                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GorgonGazeHit") with
                {
                    PitchVariance = 0.1f
                }, target.Center);

                return;
            }

            if (target.GetGlobalNPC<DebuffNPC>().soulPetrificationImmunity > 0 && 
                !ignoreCooldown)
            {
                return;
            }

            int time = 120;
            float lifePercent = (float)target.life / (float)target.lifeMax;
            // The lower the NPC's health, the longer it gets petrified
            time += (int)((1f - lifePercent) * 600f);

            if (target.boss && !ignoreBossResistance)
            {
                time /= 2;
                if (lifePercent > 0.5f)
                {
                    return;
                }
            }
            int finalTime = (int)(time * potency);

            DebuffNPC debuffs = target.GetGlobalNPC<DebuffNPC>();
            debuffs.previousNoTileCollideState = target.noTileCollide;
            if (!ignoreCooldown)
            {
                debuffs.soulPetrificationImmunity =
                    DebuffNPC.maxSoulPetrificationImmunity + finalTime;
            }
            target.AddBuff(ModContent.BuffType<SoulPetrification>(), finalTime);

            if (finalTime > 0)
            {
                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GorgonGazeHit") with
                {
                    PitchVariance = 0.1f
                }, target.Center);
            }

            // soulPetrificationImmunity = maxSoulPetrificationImmunity;
        }

        public override bool CanHitPlayer(Player target)
        {
            // Only hit entities facing towards it
            if (target.direction == Projectile.direction || 
                target.GetModPlayer<PlayerDebuffs>().soulPetrifiedImmunity > 0)
            {
                return false;
            }
            return base.CanHitPlayer(target);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            PlayerDebuffs debuff = target.GetModPlayer<PlayerDebuffs>();
            if (debuff.soulPetrifiedImmunity <= 0)
            {
                target.AddBuff(ModContent.BuffType<SoulPetrification>(), 300);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float maxSeparation = float.DegreesToRadians(5);
            float worthless = 0;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + (Projectile.velocity * gazeLength), 5, ref worthless) ||
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + (Projectile.velocity.RotatedBy(maxSeparation) * gazeLength), 
                5, ref worthless) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + (Projectile.velocity.RotatedBy(-maxSeparation) * gazeLength),
                5, ref worthless) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + (Projectile.velocity.RotatedBy(maxSeparation / 2) * gazeLength),
                5, ref worthless) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center, Projectile.Center + (Projectile.velocity.RotatedBy(-maxSeparation / 2) * gazeLength),
                5, ref worthless))
            {
                return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle bounds = gazeBeam.Value.Bounds;
            bounds.Height /= maxGorgonGazeBeamFrames;
            bounds.Y = frame * bounds.Height;
            Color color = redden ? new Color(1, 0.725f, 0.725f) : Color.White;
            if (frame < 2 || frame >= maxGorgonGazeBeamFrames - 2)
            {
                color *= 0.2f;
            } else
            {
                color *= 0.4f;
            }

            // Main.instance.PrepareDrawnEntityDrawing(Projectile, shaderID, null);

            Main.EntitySpriteDraw(
                gazeBeam.Value,
                Projectile.Center - Main.screenPosition,
                bounds,
                color,
                Projectile.rotation,
                new(20, bounds.Size().Y * 0.5f),
                1f,
                SpriteEffects.None
            );

            return false;
        }
    }
}
