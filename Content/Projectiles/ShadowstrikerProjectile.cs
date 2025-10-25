using GvMod.Common.Systems;
using GvMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ReLogic.Utilities;

namespace GvMod.Content.Projectiles
{
    public class ShadowstrikerProjectile : ModProjectile
    {
        private int Side { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        private int distance = 0;
        private int othersFrames = 0;
        private int extrasFrames = 0;
        private int animationTimer = 0;
        private SlotId soundID;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(84);
            Projectile.light = 1f;
            Projectile.scale = 1f;
            // Main.projFrames[Projectile.type] = 4;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 45;
            Projectile.knockBack = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 200;

            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
            Projectile.ownerHitCheck = false;
            Projectile.netImportant = true;
        }

        // It doesn't matter what texture it uses, as it returns false in predraw
        public override string Texture => "GvMod/Assets/Skills/Default";

        public override void OnSpawn(IEntitySource source)
        {
            // TODO: Find more appropiate sounds
            soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/DragonsphereUse") with
            {
                PitchVariance = 0.1f,
                Volume = 0.75f
            }, Projectile.Center);
        }

        public override void AI()
        {
            // Find a way to modify the distance when the player presses up or down?
            if (Main.myPlayer == Projectile.owner)
            {
                if (!Main.LocalPlayer.DeadOrGhost)
                {
                    if (distance < 120) distance++;
                    
                    if (Side >= 0)
                    {
                        Projectile.Center = Main.LocalPlayer.Center + new Vector2(distance, 0);
                    } else
                    {
                        Projectile.Center = Main.LocalPlayer.Center - new Vector2(distance, 0);
                    }
                    Projectile.netUpdate = true;
                }
            }

            if (animationTimer++ > 6)
            {
                animationTimer = 0;

                if (othersFrames++ > 2)
                {
                    othersFrames = 0;
                }

                if (extrasFrames++ > 4)
                {
                    extrasFrames = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (PlayerRenderTarget.canUseTarget)
            {
                SpriteBatchState tempState = SpriteBatchExt.GetState(Main.spriteBatch);

                SpriteBatchExt.Restart(Main.spriteBatch, tempState, SpriteSortMode.Immediate);

                Rectangle playerRect = PlayerRenderTarget.
                    getPlayerTargetSourceRectangle(Projectile.owner);
                Rectangle sourceRectangle = new Rectangle(Projectile.owner * playerRect.Width, 0,
                    playerRect.Width, playerRect.Height);

                Main.spriteBatch.Draw(PlayerRenderTarget.Target, Projectile.Center - Main.screenPosition -
                    playerRect.Size() / 2 - new Vector2(10, 21), sourceRectangle, new Color(77, 242, 229) * 0.5f);

                SpriteBatchExt.Restart(Main.spriteBatch, tempState);
            }

            if (animationTimer < 3)
            {
                Texture2D others = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/Shadowstriker_Extras").Value;
                Rectangle othersBounds = new Rectangle(96 * othersFrames, 0, 96, 96);
                Texture2D extras = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/Shadowstriker_Extras_Extras").Value;
                Rectangle extrasBounds = new Rectangle(118 * extrasFrames, 0, 118, 122);

                Main.EntitySpriteDraw(
                    others,
                    Projectile.Center - Main.screenPosition,
                    othersBounds,
                    Color.White,
                    Projectile.rotation,
                    othersBounds.Size() / 2,
                    1f,
                    SpriteEffects.None
                );

                Main.EntitySpriteDraw(
                    extras,
                    Projectile.Center - Main.screenPosition,
                    extrasBounds,
                    Color.White,
                    Projectile.rotation,
                    extrasBounds.Size() / 2,
                    1f,
                    SpriteEffects.None
                );
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ActiveSound soundInstance;
            SoundEngine.TryGetActiveSound(soundID, out soundInstance);

            if (soundInstance == null)
            {
                soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/DragonsphereConstant") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.75f
                }, Projectile.Center);
            }
            else
            {
                if (!soundInstance.IsPlaying)
                {
                    soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/DragonsphereConstant") with
                    {
                        PitchVariance = 0.1f,
                        Volume = 0.75f
                    }, Projectile.Center);
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
