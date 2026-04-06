using GvMod.Common;
using GvMod.Common.Players;
using GvMod.Common.Players.Sevenths;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Content.Projectiles
{
    public enum KunaiSummon
    {
        None, 
        Scorpion, 
        Bee,
        Bat,
        Hornet,
        BigBat, 
        HellBat, 
        IceBat, 
        Crimera, 
        EaterOfSouls, 
        Pixie, 
        FlyingSnake, 
        Pigron, 
        Slime, 
        FlyingEye, 
        WanderingEye
    }

    public record struct SummonStats(int projectileID, int baseDamage, float baseKnockback, 
        List<Condition> conditions);

    public class Kunai : ModProjectile
    {
        public static Asset<Texture2D> kunaiExtras;
        public static Asset<Texture2D> kunaiInnerGlow;
        int visualFrame = 0;
        int visualCounter = 0;
        public KunaiSummon SummonType { get => (KunaiSummon)Projectile.ai[0]; set => Projectile.ai[0] = (int)value; }
        public bool SuperKunai { get => Projectile.ai[1] == 1; set => Projectile.ai[1] = value ? 0 : 1; }
        public float CostMod { get => Projectile.ai[2]; set => Projectile.ai[2] = value; }

        public override void SetStaticDefaults()
        {
            kunaiExtras = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/SuperKunaiExtras_Back");
            kunaiInnerGlow = ModContent.Request<Texture2D>("GvMod/Content/Projectiles/SuperKunaiExtras");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.scale = 1f;
            Projectile.light = 0f;

            Projectile.DamageType = ModContent.GetInstance<MainAttackDamage>();
            Projectile.damage = 12;
            Projectile.knockBack = 0;
            Projectile.extraUpdates = 2;

            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 240;
            Projectile.ownerHitCheck = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Light up at certain power level
            float vol = 1;
            if (SuperKunai)
            {
                Projectile.light = 0.75f;
                vol = 1.75f;
            }

            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/KunaiThrow") with
            {
                PitchVariance = 0.1f,
                Volume = vol
            }, Projectile.Center);
            /*Main.NewText("Spawned with type: " + SummonType.ToString());
            Main.NewText("Super: " + SuperKunai);
            Main.NewText("Cost multiplier: " + CostMod);*/
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() > 0) Projectile.rotation = Projectile.velocity.ToRotation();

            ExtrasTimer();
        }

        public void ExtrasTimer()
        {
            if (visualCounter >= 8)
            {
                visualFrame++;
                visualCounter = 0;
                if (visualFrame >= 3)
                {
                    visualFrame = 0;
                }
            }

            visualCounter++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly || target.immortal || !target.active) return;

            if (Main.myPlayer == Projectile.owner)
            {
                SeptimaPlayer adept = Main.player[Projectile.owner].GetModPlayer<SeptimaPlayer>();

                if (target.active && target.life > 0 && !target.immortal && !target.friendly &&
                    adept.septimaType == SeptimaType.Rebirth && SummonType != KunaiSummon.None)
                {
                    int timer = 900;
                    Tag newTag = adept.TaggedNPCs.AddTag(target.whoAmI, timer);

                    if (adept.TaggedNPCs.GetTag(target.whoAmI).tagLevel >= 3)
                    {
                        SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/FullTagSound") with
                        {
                            Volume = 0.5f,
                            PitchVariance = 0.1f
                        }, Projectile.Center);
                    }
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/TagSound") with
                        {
                            Volume = 0.5f,
                            PitchVariance = 0.1f
                        }, Projectile.Center);
                    }

                    float extra = adept.septima.GetTagSkillPower(Main.LocalPlayer, adept, newTag);
                    if (SuperKunai)
                    {
                        extra += 0.15f;
                    }
                    // Add tag damage to potency when hit an NPC
                    SummonStats stats = Rebirth.ProjectileTable[SummonType];
                    bool? res = (adept.septima as Rebirth)?.RaiseUndead(SummonType, Projectile.Center,
                        Main.LocalPlayer, adept, extra, CostMod);
                    //Main.NewText("Spawned from hitting enemy", Color.Yellow);

                    if (!(bool)res)
                    {
                        foreach (int key in Rebirth.CorpseItemTable.Keys)
                        {
                            if (Rebirth.CorpseItemTable[key] == SummonType)
                            {
                                Item.NewItem(Projectile.GetSource_DropAsItem(),
                                    new Rectangle((int)Projectile.position.X,
                                    (int)Projectile.position.Y, Projectile.width,
                                    Projectile.height), key);
                                break;
                            }
                        }
                    }
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public Vector2 GetEmptySpaces()
        {
            Vector2 projectileVelocity = new Vector2(-1, 0);
            int[] directionValues = { 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < directionValues.Length; i++)
            {
                Vector2 direction = new Vector2(0, -1).RotatedBy(-MathHelper.PiOver4 * i);
                for (int k = -2; k < 3; k++)
                {
                    Vector2 tempDirection = direction.RotatedBy(MathHelper.PiOver4 * k);
                    if (!Collision.SolidCollision(Projectile.Center + (tempDirection * 16), 1,
                        1))
                    {
                        directionValues[i]++;
                    }
                }
            }

            int highestValue = 0;
            int index = 0;
            for (int i = 0; i < directionValues.Length; i++)
            {
                if (directionValues[i] > highestValue)
                {
                    highestValue = directionValues[i];
                    index = i;
                }
            }

            projectileVelocity = new Vector2(0, -1).RotatedBy(-MathHelper.PiOver4 * index);
            return projectileVelocity;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.HasBuff(BuffID.Stoned))
            {
                modifiers.SourceDamage += 1;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff(BuffID.Stoned))
            {
                modifiers.SourceDamage += 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> field = TextureAssets.Projectile[Type];
            Rectangle bounds = field.Frame();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            if (SuperKunai)
            {
                SpriteBatchState prevState = SpriteBatchExt.GetState(Main.spriteBatch);
                SpriteBatchExt.Restart(Main.spriteBatch, prevState, SpriteSortMode.Immediate);
                MiscShaderData shader = GameShaders.Misc["GvMod:Rebirth"];

                int otherFrame = visualFrame + 1;
                if (otherFrame >= 3)
                {
                    otherFrame -= 3;
                }

                bounds = kunaiExtras.Frame(1, 3, 0, otherFrame);
                DrawData backAura = new DrawData(
                    kunaiExtras.Value,
                    drawPos,
                    bounds,
                    new Color(135, 100, 155) * Projectile.Opacity * 0.5f,
                    Projectile.rotation,
                    bounds.Size() / 2f,
                    Projectile.scale,
                    SpriteEffects.None
                    );

                bounds = kunaiExtras.Frame(1, 3, 0, visualFrame);
                DrawData frontAura = new DrawData(
                    kunaiExtras.Value,
                    drawPos,
                    bounds,
                    Color.White * Projectile.Opacity * 0.8f,
                    Projectile.rotation,
                    bounds.Size() / 2f,
                    Projectile.scale,
                    SpriteEffects.None
                    );

                shader.Apply(backAura);
                shader.Apply(frontAura);

                Main.EntitySpriteDraw(backAura);
                Main.EntitySpriteDraw(frontAura);

                SpriteBatchExt.Restart(Main.spriteBatch, prevState);
            }

            Main.EntitySpriteDraw(
                field.Value,
                Projectile.Center - Main.screenPosition,
                field.Frame(),
                lightColor * Projectile.Opacity,
                Projectile.rotation,
                field.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None
            );

            if (SuperKunai)
            {
                bounds = kunaiInnerGlow.Frame(1, 3, 0, visualFrame);
                Main.EntitySpriteDraw(
                    kunaiInnerGlow.Value,
                    drawPos,
                    bounds,
                    Color.White * Projectile.Opacity,
                    Projectile.rotation,
                    bounds.Size() / 2f,
                    Projectile.scale,
                    SpriteEffects.None
                    );
            }
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (SummonType != KunaiSummon.None)
            {
                // Main.NewText("Summoning " + SummonType.ToString());
                if (Main.myPlayer == Projectile.owner)
                {
                    float extra = 1f;
                    if (SuperKunai)
                    {
                        extra += 0.15f;
                    }
                    SeptimaPlayer adept = Main.LocalPlayer.GetModPlayer<SeptimaPlayer>();
                    //Main.NewText("Spawning from hitting tile", Color.Blue);
                    bool? res = (adept.septima as Rebirth)?.RaiseUndead(SummonType, Projectile.Center,
                        Main.LocalPlayer, adept, extra, CostMod);

                    if (!(bool)res)
                    {
                        foreach (int key in Rebirth.CorpseItemTable.Keys)
                        {
                            if (Rebirth.CorpseItemTable[key] == SummonType)
                            {
                                Item.NewItem(Projectile.GetSource_DropAsItem(),
                                    new Rectangle((int)Projectile.position.X,
                                    (int)Projectile.position.Y, Projectile.width,
                                    Projectile.height), key);
                                break;
                            }
                        }
                    }
                }
            }

            SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/KunaiClash") with
            {
                PitchVariance = 0.1f
            }, Projectile.Center);

            return base.OnTileCollide(oldVelocity);
        }
    }
}
