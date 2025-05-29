using System.Collections.Generic;
using System.IO;
using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public class HairDartProjectile : ModProjectile
    {
        private Vector2 mizuchiTargetPosition = new Vector2(0, 0);
        private Vector2 mizuchiTargetDirection = new Vector2(0, 0);
        private int dartFrame = 0;
        private int dartTimer = 0;
        private int reticleFrame = 0;
        private int reticleTimer = 0;
        private List<DartLeaderUpgrades> registeredUpgrades = new();
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.scale = 1f;
            Projectile.light = 0.175f;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.damage = 1;
            Projectile.knockBack = 0;

            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.ownerHitCheck = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 3; i++)
            {
                DartLeaderUpgrades upgrade = (DartLeaderUpgrades)Projectile.ai[i];
                if (registeredUpgrades.Contains(upgrade)) continue;
                switch (upgrade)
                {
                    case DartLeaderUpgrades.Naga:
                        Projectile.penetrate = -1;
                        break;
                    case DartLeaderUpgrades.Dullahan:
                        Projectile.width = 12;
                        Projectile.height = 12;
                        Projectile.light = 0.5f;
                        break;
                    case DartLeaderUpgrades.Mizuchi:
                        mizuchiTargetPosition = Projectile.Center + (Projectile.velocity * 60);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            mizuchiTargetDirection = mizuchiTargetPosition.DirectionTo(Main.MouseWorld);
                        }
                        Projectile.netUpdate = true;
                        break;
                    case DartLeaderUpgrades.Vasuki:
                    case DartLeaderUpgrades.VasukiVisuals:
                        Projectile.tileCollide = false;
                        break;
                }
                registeredUpgrades.Add(upgrade);
            }
            base.OnSpawn(source);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(mizuchiTargetDirection);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            mizuchiTargetDirection = reader.ReadVector2();
            base.ReceiveExtraAI(reader);
        }

        public override bool? CanHitNPC(NPC target)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Projectile.ai[i] == (int)DartLeaderUpgrades.VasukiVisuals && Projectile.timeLeft >= 355)
                {
                    return false;
                }
            }
            return base.CanHitNPC(target);
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                DartLeaderUpgrades upgrade = (DartLeaderUpgrades)Projectile.ai[i];
                switch (upgrade)
                {
                    case DartLeaderUpgrades.Mizuchi:
                        if (mizuchiTargetPosition == new Vector2(-1, -1)) continue;

                        if (Main.myPlayer == Projectile.owner)
                        {
                            mizuchiTargetDirection = mizuchiTargetPosition.DirectionTo(Main.MouseWorld);
                        }

                        if (Projectile.Center.Distance(mizuchiTargetPosition) <= 24)
                        {
                            float rotationNeeded = mizuchiTargetDirection.ToRotation() - 
                                Projectile.velocity.ToRotation();
                            Projectile.velocity = Projectile.velocity.RotatedBy(rotationNeeded);
                            mizuchiTargetPosition = new Vector2(-1, -1);
                        }
                        reticleTimer++;
                        Projectile.netUpdate = true;
                        break;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            dartTimer++;
            if (dartTimer > 3)
            {
                dartFrame++;
                dartTimer = 0;
                if (dartFrame >= 2)
                {
                    dartFrame = 0;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/DartHit") with
            {
                PitchVariance = 0.1f
            }, Projectile.Center);

            if (target.friendly || target.immortal || !target.active) return;
            
            if (Main.myPlayer == Projectile.owner)
            {
                SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
                // The only septima restriction on normal weapons is that the player needs a septima to tag
                if (target.active && target.life > 0 && !target.immortal && !target.friendly && 
                    adept.septimaType != SeptimaType.None)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        DartLeaderUpgrades upgrade = (DartLeaderUpgrades)Projectile.ai[i];
                        if (upgrade == DartLeaderUpgrades.Ouroboros)
                        {
                            adept.TaggedNPCs.AddTag(target.whoAmI, 1800);
                            base.OnHitNPC(target, hit, damageDone);
                            return;
                        }
                    }
                    adept.TaggedNPCs.AddTag(target.whoAmI);
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/DartMiss") with
            {
                PitchVariance = 0.1f
            }, Projectile.Center);
            return base.OnTileCollide(oldVelocity);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            bool setMax = true;
            for (int i = 0; i < 3; i++)
            {
                DartLeaderUpgrades upgrade = (DartLeaderUpgrades)Projectile.ai[i];
                // Dullahan allows the projectile to deal more than 1 damage
                if (upgrade == DartLeaderUpgrades.Dullahan)
                {
                    setMax = false;
                }
            }
            if (setMax) modifiers.SetMaxDamage(1);
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == (int)DartLeaderUpgrades.Dullahan || 
                Projectile.ai[1] == (int)DartLeaderUpgrades.Dullahan ||
                Projectile.ai[2] == (int)DartLeaderUpgrades.Dullahan)
            {
                Asset<Texture2D> dullahanTexture = ModContent.
                    Request<Texture2D>("GvMod/Content/Projectiles/DullahanBolt");
                Main.EntitySpriteDraw(
                    dullahanTexture.Value,
                    Projectile.Center - Main.screenPosition,
                    new Rectangle(56 * dartFrame, 0, 56, 38),
                    lightColor,
                    Projectile.rotation,
                    new Vector2(28, 19),
                    1,
                    SpriteEffects.None
                );

                return false;
            }

            Point centerPoint = Projectile.Center.ToTileCoordinates();

            int currentVisuals = (int)DartLeaderUpgrades.None;
            Color selectedColor = Lighting.GetColor(centerPoint, new Color(29, 210, 210));

            Asset<Texture2D> coloredTexture = ModContent.
                Request<Texture2D>("GvMod/Content/Projectiles/HairDartProjectile_Colored");
            Asset<Texture2D> centerTexture = ModContent.
                Request<Texture2D>("GvMod/Content/Projectiles/HairDartProjectile");

            for (int i = 0; i < 3; i++)
            {
                if (Projectile.ai[i] > currentVisuals)
                {
                    currentVisuals = (int)Projectile.ai[i];
                    switch ((DartLeaderUpgrades)Projectile.ai[i])
                    {
                        default:
                        case DartLeaderUpgrades.None:
                            selectedColor = Lighting.GetColor(centerPoint, new Color(29, 210, 210));
                            break;
                        case DartLeaderUpgrades.Naga:
                            selectedColor = Lighting.GetColor(centerPoint, new Color(26, 10, 204));
                            break;
                        case DartLeaderUpgrades.Technos:
                            selectedColor = Lighting.GetColor(centerPoint, new Color(18, 255, 18));
                            break;
                        case DartLeaderUpgrades.Orochi:
                            selectedColor = Lighting.GetColor(centerPoint, new Color(255, 213, 0));
                            break;
                        case DartLeaderUpgrades.Mizuchi:
                            selectedColor = Lighting.GetColor(centerPoint, new Color(255, 13, 13));
                            break;
                        case DartLeaderUpgrades.Ouroboros:
                            selectedColor = Lighting.GetColor(centerPoint, new Color(64, 255, 150));
                            break;
                        case DartLeaderUpgrades.Vasuki:
                            selectedColor = Lighting.GetColor(centerPoint, new Color(204, 0, 190));
                            break;
                        case DartLeaderUpgrades.VasukiVisuals:
                            selectedColor = Lighting.GetColor(centerPoint, new Color(204, 0, 190));
                            break;
                    }
                }
            }

            //Main.NewText("Color: " + selectedColor);
            //Main.NewText("Id: " + currentVisuals);

            Main.EntitySpriteDraw(
                coloredTexture.Value,
                Projectile.Center - Main.screenPosition,
                new Rectangle(42 * dartFrame, 0, 42, 26),
                selectedColor,
                Projectile.rotation,
                new Vector2(21, 13),
                1,
                SpriteEffects.None
            );

            Main.EntitySpriteDraw(
                centerTexture.Value,
                Projectile.Center - Main.screenPosition,
                new Rectangle(42 * dartFrame, 0, 42, 26),
                lightColor,
                Projectile.rotation,
                new Vector2(21, 13),
                1,
                SpriteEffects.None
            );
            return false;
        }

        public override bool PreDrawExtras()
        {
            // Draw mizuchi reticle
            bool reticleDrawn = false;
            for (int i = 0; i < 3; i++)
            {
                DartLeaderUpgrades upgrade = (DartLeaderUpgrades)Projectile.ai[i];
                // Dullahan allows the projectile to deal more than 1 damage
                if (upgrade == DartLeaderUpgrades.Mizuchi && !reticleDrawn)
                {
                    if (reticleTimer >= 3)
                    {
                        reticleFrame++;
                        reticleTimer = 0;
                        if (reticleFrame > 2)
                        {
                            reticleFrame = 0;
                        }
                    }

                    if (reticleTimer%3 == 0 || reticleTimer == 0)
                    {
                        reticleDrawn = true;
                        continue;
                    }

                    Asset<Texture2D> reticleTexture = ModContent.Request<Texture2D>("GvMod/Assets/Effects/MizuchiReticle");
                    Main.EntitySpriteDraw(
                        reticleTexture.Value,
                        mizuchiTargetPosition - Main.screenPosition,
                        new Rectangle(44 * reticleFrame, 0, 44, 36),
                        Color.White * 0.75f,
                        mizuchiTargetDirection.ToRotation(), 
                        new Vector2(20, 18), 
                        1, 
                        SpriteEffects.None
                    );
                    reticleDrawn = true;
                }
            }
            return base.PreDrawExtras();
        }
    }
}
