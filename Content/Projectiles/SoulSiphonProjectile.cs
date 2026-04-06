using GvMod.Common.Players.Skills;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    class SoulSiphonProjectile : ModProjectile
    {
        int timePassed = 0;
        static int initialRange = 32;
        int size = 0;
        SlotId soundID;

        public override void SetDefaults()
        {
            Projectile.width = 826;
            Projectile.height = 826;
            Projectile.scale = 1f;

            Projectile.DamageType = ModContent.GetInstance<SpecialAttackDamage>();
            Projectile.damage = 0;
            Projectile.knockBack = 1;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            size = initialRange;
            soundID = SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/SoulSiphon") with
            {
                Volume = 1f,
                PitchVariance = 0.1f
            }, Projectile.Center, SoundPermanence);
            UpdateSize();
        }

        public bool SoundPermanence(ActiveSound sound)
        {
            return Projectile.active && Projectile.ModProjectile is SoulSiphonProjectile;
        }


        public override void AI()
        {
            UpdateSize();

            Projectile.Center = Main.player[Projectile.owner].Center;
            timePassed++;
            Projectile.netUpdate = true;

            if (Projectile.timeLeft % 3 == 0)
            {
                int maxDust = 4;
                maxDust += size / 50;
                int dust = Main._rand.Next(0, maxDust);
                for (int i = 0; i < dust; i++)
                {
                    Dust.NewDust(Projectile.Center - Projectile.Size / 2, size, size,
                        DustID.PurpleTorch);
                }
            }

            if (Projectile.timeLeft <= 2 && Projectile.Opacity > 0)
            {
                Projectile.timeLeft++;
                Projectile.Opacity -= 0.025f;
                SoundEngine.TryGetActiveSound(soundID, out ActiveSound sound);
                if (sound != null)
                {
                    if (sound.Volume > 0)
                    {
                        sound.Volume -= 1f / 15f;
                    }
                }
            }
        }

        public void UpdateSize()
        {
            size = initialRange + (int)(Easing.EaseInExponential(
                timePassed, SoulSiphon.MaxSoulSiphonAttackTime) * SoulSiphon.MaxSoulSiphonRange);
            Projectile.width = size;
            Projectile.height = size;
            Projectile.scale = (float)size / 826f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> field = TextureAssets.Projectile[Type];

            Main.EntitySpriteDraw(
                field.Value,
                Projectile.Center - Main.screenPosition,
                field.Frame(),
                Color.White * Projectile.Opacity,
                0,
                field.Size() * 0.5f,
                Projectile.scale, 
                SpriteEffects.None
            );

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
            /*Vector2 halfSize = projHitbox.Size() / 2f;
            halfSize *= Projectile.scale;
            Vector2 ellipsePosition = projHitbox.Center.ToVector2() - halfSize;
            Vector2 ellipseDimentions = projHitbox.Center.ToVector2() + halfSize;
            Vector2 ellipseCenter = projHitbox.Center.ToVector2();
            float x = 0f;
            float y = 0f;
            if (targetHitbox.Left > ellipseCenter.X)
            {
                x = targetHitbox.Left - ellipseCenter.X;
            }
            else if (targetHitbox.Left + targetHitbox.Width < ellipseCenter.X)
            {
                x = targetHitbox.Left + targetHitbox.Width - ellipseCenter.X;
            }
            if (targetHitbox.Top > ellipseCenter.Y)
            {
                y = targetHitbox.Top - ellipseCenter.Y;
            }
            else if (targetHitbox.Top + targetHitbox.Height < ellipseCenter.Y)
            {
                y = targetHitbox.Top + targetHitbox.Height - ellipseCenter.Y;
            }
            float a = ellipseDimentions.X / 2f;
            float b = ellipseDimentions.Y / 2f;
            return (x * x) / (a * a) + (y * y) / (b * b) <= 1;*/
        }
    }
}
