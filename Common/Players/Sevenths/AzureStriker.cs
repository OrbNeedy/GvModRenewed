using System;
using System.Collections.Generic;
using GvMod.Common.GlobalNPCs;
using GvMod.Common.Players.Skills;
using GvMod.Common.Utils;
using GvMod.Content;
using GvMod.Content.Buffs;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Sevenths
{
    public class AzureStriker : Septima
    {
        // Septima uniques
        public bool activeFlashfield = false;
        public int flashfieldIndex = -1;
        public bool[] activeSpheres = { false, false, false };
        public int[] spheresIndexes = { -1, -1, -1 };
        public float sphereBaseRotation = 0;
        public int pulsarBonusTimer = 0;
        public const int maxPulsarBonusTimer = 1200;

        public int attackFrame = 0;
        public int attackTimer = 0;
        public float attackRotation = 0;

        public int ArmedPhenomenonClawCooldown = 0;

        public override float BaseBasicAttackDamage { get; protected set; } = 7;
        public override float BaseSecondaryAttackDamage { get; protected set; } = 20;
        public override List<SpecialSkill> SkillList { get; protected set; } = new() { new SpecialSkill(),
            new Astrasphere(), new GalvanicPatch(), new Luxcalibur(), new VoltaicChains(), new AlchemicalField(), 
            new InfiniteSurge(), new GalvanicRenewal(), new SeptimalBurst(), new SeptimalShield(), 
            new SeptimalSurge(), new SplitSecond(), new GrandStrizer(), new Dragonsphere().SetUnlockConditions(
                [CustomConditions.FirstDragonVein]), new GFree(), new Electroshock().
            SetUnlockConditions([CustomConditions.FifthDragonVein]), new Shadowstriker().
            SetUnlockConditions([CustomConditions.ThirdDragonVein])
        };
        public override float EPUseBase { get; protected set; } = 0.75f;
        public override float EPRecoveryBaseRate { get; protected set; } = 1f / 210f;
        public override int EPCooldownBaseTimer { get; protected set; } = 90;
        public override float OverheatRecoveryBaseRate { get; protected set; } = 1f / 420f;
        public override float SPRecoveryBaseRate { get; protected set; } = 1f / 5400f;
        public override int PrevasionEPCooldownBaseTimer { get; protected set; } = 90;

        public override SeptimaType Type { get; protected set; } = SeptimaType.AzureStriker;
        public override string InternalName => "AzureStriker";
        public override Color MainColor => new Color(77, 242, 229);

        public override void PostLoadSeptima(Mod mod)
        {
            int headID = EquipLoader.GetEquipSlot(mod, "AzureStrikerArmedPhenomenon", EquipType.Head);
            ArmorIDs.Head.Sets.DrawFullHair[headID] = true;
        }

        public override Dictionary<int, Resistance> GetNPCResistances()
        {
            return new()
            {
                [NPCID.WaterSphere] = Resistance.Penetrate,
                [NPCID.Sharkron] = Resistance.Penetrate,
                [NPCID.Sharkron2] = Resistance.Penetrate
            };
        }

        public override Dictionary<int, Resistance> GetProjectileResistances()
        {
            return new()
            {
                [ProjectileID.WaterBolt] = Resistance.Overheat,
                [ProjectileID.WaterGun] = Resistance.Penetrate,
                [ProjectileID.WaterStream] = Resistance.Overheat,
                [ProjectileID.BloodWater] = Resistance.Penetrate,
                [ProjectileID.HolyWater] = Resistance.Penetrate,
                [ProjectileID.UnholyWater] = Resistance.Penetrate,
                [ProjectileID.Electrosphere] = Resistance.Absorb,
                [ProjectileID.ElectrosphereMissile] = Resistance.Absorb,
                [ProjectileID.ThunderSpear] = Resistance.Absorb,
                [ProjectileID.ThunderSpearShot] = Resistance.Absorb,
                [ProjectileID.ThunderStaffShot] = Resistance.Ignore,
                [ProjectileID.MartianTurretBolt] = Resistance.Absorb,
                [ProjectileID.CultistBossLightningOrbArc] = Resistance.Ignore,
                [ProjectileID.CultistBossLightningOrb] = Resistance.Penetrate,
                [ProjectileID.MedusaHead] = Resistance.Overheat,
                [ProjectileID.MedusaHeadRay] = Resistance.Overheat,
                [ProjectileID.Sharknado] = Resistance.Overheat,
                [ProjectileID.Cthulunado] = Resistance.Overheat,
                [ModContent.ProjectileType<Flashfield>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<FlashphereProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<AstrasphereProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<AstrasphereOrbits>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<LuxcaliburProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<VoltaicChainProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<GrandStrizerProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<WideThunder>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<SoulSiphonExplosion>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<GorgonGazeBeam>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<GorgoneiaBeam>()] = Resistance.Penetrate
            };
        }

        public override void InitializeSeptima(Player player, SeptimaPlayer adept, Mod mod)
        {
        }

        public override void LoadSeptima(Mod mod)
        {
            int headID = EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/" +
                $"Glaive_Head_AzureStriker", EquipType.Head, name: "AzureStrikerArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_Body_AzureStriker",
                EquipType.Body, name: "AzureStrikerArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_Legs_AzureStriker",
                EquipType.Legs, name: "AzureStrikerArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_Wings_AzureStriker",
                EquipType.Wings, name: "AzureStrikerArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_Waist_AzureStriker",
                EquipType.Waist, name: "AzureStrikerArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_HandsOff_AzureStriker",
                EquipType.HandsOff, name: "AzureStrikerArmedPhenomenon");
            EquipLoader.AddEquipTexture(mod, $"GvMod/Assets/ArmedPhenomena/Glaive_HandsOn_AzureStriker",
                EquipType.HandsOn, name: "AzureStrikerArmedPhenomenon");
        }

        public override void MiscEffects(Player player, SeptimaPlayer adept)
        {
            if (flashfieldIndex > -1)
            {
                Projectile flashfield = Main.projectile[flashfieldIndex];
                activeFlashfield = flashfield.active && (flashfield.ModProjectile is Flashfield || 
                    flashfield.ModProjectile is FlashphereProjectile) && 
                    flashfield.owner == player.whoAmI;
            } else
            {
                activeFlashfield = false;
            }

            AllowPrevasion = !activeFlashfield && (!player.wet || adept.Stage >= 5);

            player.buffImmune[BuffID.Electrified] = true;
        }

        public override void MovementEffects(Player player, SeptimaPlayer adept)
        {
            player.maxRunSpeed *= 1.075f;
            player.runAcceleration *= 1.2f;

            if (adept.DnizerMode)
            {
                player.GetModPlayer<PlayerBuffs>().FreeFloat = true;
            }
        }

        public override bool MainSkillUse(Player player, SeptimaPlayer adept)
        {
            // Main.NewText("Frames: " + adept.MainSkillUseTime);
            if (player.wet)
            {
                if (adept.Stage < 5)
                {
                    if (adept.ForceOverheat()) return true;
                } else if (adept.Stage <= 7)
                {
                    adept.EPUseModifier *= 2;
                }
            }

            if (adept.MainSkillUseTime <= 0)
            {
                adept.CurrentEP -= adept.GetTotalMaxEP() * 0.08f * adept.GetTotalEPUseModifier();
            }

            if ((!activeFlashfield || flashfieldIndex == -1) && Main.myPlayer == player.whoAmI)
            {
                if (adept.DnizerMode)
                {
                    int finalDamage = (int)player.GetTotalDamage<MainAttackDamage>().
                        ApplyTo(GetBasicSkillPower(player, adept));
                    flashfieldIndex = Projectile.NewProjectile(player.GetSource_FromThis("Septima"),
                        player.Center, Vector2.Zero, ModContent.ProjectileType<FlashphereProjectile>(), 
                        finalDamage, 3, player.whoAmI, (int)AstraspheredBehavior.Follow);
                } else
                {
                    int finalDamage = (int)player.GetTotalDamage<MainAttackDamage>().
                        ApplyTo(GetBasicSkillPower(player, adept));
                    flashfieldIndex = Projectile.NewProjectile(player.GetSource_FromThis("Septima"),
                        player.Center, Vector2.Zero, ModContent.ProjectileType<Flashfield>(), finalDamage, 0,
                        player.whoAmI);
                    // Main.NewText("Damage applied: " + finalDamage);
                }
            }

            Projectile flashfield = Main.projectile[flashfieldIndex];
            if (adept.DnizerMode) 
            {
                activeFlashfield = flashfield.active && flashfield.ModProjectile is FlashphereProjectile &&
                    flashfield.owner == player.whoAmI;
            } else
            {
                activeFlashfield = flashfield.active && flashfield.ModProjectile is Flashfield &&
                    flashfield.owner == player.whoAmI;
            }

            // Reset timer and assert friendlyness
            if (activeFlashfield)
            {
                flashfield.timeLeft = 6;
                flashfield.friendly = true;
                flashfield.hostile = false;
                flashfield.netUpdate = true;
            }
            
            if (adept.DnizerMode)
            {
                for (int i = 0; i < activeSpheres.Length; i++)
                {
                    bool flag = activeSpheres[i];
                    int index = spheresIndexes[i];

                    if ((!flag || index == -1) && Main.myPlayer == player.whoAmI && activeFlashfield)
                    {
                        int finalDamage = (int)player.GetTotalDamage<MainAttackDamage>().
                            ApplyTo(GetBasicSkillPower(player, adept));
                        index = spheresIndexes[i] = Projectile.NewProjectile(
                            player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero, 
                            ModContent.ProjectileType<AstrasphereOrbits>(), finalDamage, 3, player.whoAmI, 
                            flashfieldIndex, sphereBaseRotation + (MathHelper.TwoPi * i / 3));
                    }

                    Projectile sphere = Main.projectile[index];
                    flag = activeSpheres[i] = sphere.active && sphere.ModProjectile is AstrasphereOrbits &&
                            sphere.owner == player.whoAmI;

                    if (activeSpheres[i])
                    {
                        // Main.NewText("Asserting time for sphere " + activeSpheres[i]);
                        sphere.timeLeft = 6;
                        sphere.friendly = true;
                        sphere.hostile = false;
                        sphere.netUpdate = true;
                    }
                }
            }

            // Give player fall immunity
            player.noFallDmg = true;
            if (!player.GetModPlayer<PlayerBuffs>().FreeFloat)
            {
                player.maxFallSpeed *= 0.25f;
            }
            player.fallStart = (int)player.Center.Y;

            return true;
        }

        public override int TagEffect(Player player, SeptimaPlayer adept, int index, ref NPCTags tags)
        {
            Tag tag = tags.GetTagByIndex(index);
            float knockback = 0;
            if (adept.MainSkillUseTime <= 0)
            {
                knockback = 2.5f;
            }

            // Tell the taggedNPC to show damage effects
            NPC target = Main.npc[tag.targetIndex];
            target.GetGlobalNPC<TagNPC>().attacked = true;
            target.GetGlobalNPC<TagNPC>().framesUntilReset = 2;

            if (tag.damageTimer > 0) return 0;

            // Damage gets reduced if the player has too many tags
            int finalDamage = (int)player.GetTotalDamage<MainAttackDamage>().
                ApplyTo(GetTagSkillPower(player, adept, tag, adept.TaggedNPCs.targetCount));
            int direction = 1;
            if ((target.Center.X - player.Center.X) < 0)
            {
                direction = -1;
            }

            bool crit = player.GetTotalCritChance<MainAttackDamage>() < Main.rand.NextFloat();
            /*player.ApplyDamageToNPC(target, finalDamage, knockback, direction, crit,
                ModContent.GetInstance<MainAttackDamage>(), true);*/
            int returnDamage = ApplyDamageToNPCAndReturnFinalDamage(player, target, finalDamage, knockback, 
                direction, crit, ModContent.GetInstance<MainAttackDamage>(), true);
            // player.Hurt(new PlayerDeathReason(), finalDamage, direction, true, dodgeable: false);
            if (returnDamage > 0)
            {
                tags.damageTimer[index] = 10;
            }

            return returnDamage;
        }

        public override int SecondarySkillUse(Player player, SeptimaPlayer adept)
        {
            if (adept.SecondarySkillUseTime == 0 && Main.myPlayer == player.whoAmI)
            {
                SoundEngine.PlaySound(new SoundStyle("GvMod/Assets/Sfx/GlobalSpecialSkillUse") with
                {
                    PitchVariance = 0.1f,
                    Volume = 0.75f
                }, player.Center);

                int finalDamage = (int)player.GetTotalDamage<SecondaryAttackDamage>().
                    ApplyTo(GetSecondarySkillPower(player, adept));
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero, 
                    ModContent.ProjectileType<Thunder>(), finalDamage, 0, player.whoAmI, 1);
            }
            return adept.SecondarySkillUseTime >= 60 ? 480 : 0;
        }

        public override void OnLevelUp(Player player, SeptimaPlayer adept)
        {
            base.OnLevelUp(player, adept);
        }

        public override void DrawAttack(ref PlayerDrawSet drawInfo, Player player, SeptimaPlayer adept)
        {
            if (drawInfo.shadow == 0f && activeFlashfield)
            {
                if (adept.MainSkillUseTime <= 0 || attackTimer % 2 == 0) return;

                Asset<Texture2D> thunder = ModContent.
                    Request<Texture2D>("GvMod/Content/Projectiles/ReachingLightning");

                for (int i = 0; i < adept.TaggedNPCs.targetCount; i++)
                {
                    if (i >= 9) break;
                    Tag currentTag = adept.TaggedNPCs.GetTagByIndex(i);
                    NPC target = Main.npc[currentTag.targetIndex];
                    float totalDistance = player.Center.Distance(target.Center);
                    Vector2 baseDirection = new Vector2(0, -1).
                        RotatedBy((MathHelper.TwoPi / adept.TaggedNPCs.targetCount * i));
                    Vector2 currentPosition = player.MountedCenter;
                    float alpha = 1f;
                    //Main.NewText("Target: " + target.FullName, MainColor);
                    //Main.NewText("ID: " + i, MainColor);
                    // 22x20
                    for (int k = 0; k < 10; k++)
                    {
                        //Main.NewText("Segment: " + k, new Color(255, 0, 0));
                        //Main.NewText("Distance: " + totalDistance, new Color(255, 0, 0));

                        if (k >= 6)
                        {
                            alpha -= 0.25f;
                        }
                        int finalFrame = attackFrame + k;
                        if (finalFrame > 6) finalFrame = 0;
                        Rectangle bounds = new Rectangle(140*finalFrame, 0, 140, 76);

                        Vector2 targetDirection = currentPosition.DirectionTo(target.Center);

                        Vector2 nextDirection = Vector2.Lerp(baseDirection, targetDirection, 
                            0.8f/((totalDistance*0.0005f)+0.001f));
                        float rotationToNext = currentPosition.DirectionTo(currentPosition+nextDirection).ToRotation();

                        if (currentPosition.Distance(target.Center) <= 128)
                        {
                            rotationToNext = currentPosition.DirectionTo(target.Center).ToRotation();
                            bounds.Width = (int)(140 * (currentPosition.Distance(target.Center)/128));
                        }

                        drawInfo.DrawDataCache.Add(
                            new DrawData(
                                thunder.Value,
                                currentPosition - Main.screenPosition,
                                bounds,
                                Color.White * alpha,
                                rotationToNext,
                                new Vector2(2, 38),
                                1f,
                                SpriteEffects.None
                        ));

                        if (currentPosition.Distance(target.Center) <= 128) break;

                        nextDirection.Normalize();

                        baseDirection = nextDirection;
                        currentPosition += baseDirection * 128f;
                    }
                }
            }
            base.DrawAttack(ref drawInfo, player, adept);
        }

        public override void OnDnizerActive(Player player, SeptimaPlayer adept)
        {
            if (Main.myPlayer != player.whoAmI) return;

            for (int i = -5; i < 6; i++)
            {
                Vector2 offset = new Vector2(120 * i, 0);
                int delay = 6 * Math.Abs(i);
                int finalDamage = (int)player.GetTotalDamage<SecondaryAttackDamage>().
                    ApplyTo(GetSecondarySkillPower(player, adept));
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center - offset,
                    Vector2.Zero, ModContent.ProjectileType<Thunder>(), finalDamage, 0, player.whoAmI, 
                    delay);
            }
            base.OnDnizerActive(player, adept);
        }

        public override void OnOverheat(Player player, SeptimaPlayer adept)
        {
            if (player.GetModPlayer<SetBonusPlayer>().pulsarUpgrade)
            {
                //Main.NewText("On Overheat pulsar upgrade trigger");
                if (pulsarBonusTimer <= 0)
                {
                    adept.CurrentSP++;
                    pulsarBonusTimer = maxPulsarBonusTimer;
                }

                if (Main.rand.NextBool(10))
                {
                    //Main.NewText("Secondary effect trigger");
                    adept.CurrentEP = adept.GetTotalMaxEP();
                    adept.Overheated = false;
                }
            }
        }

        public override void OnOverheatRecovery(Player player, SeptimaPlayer adept)
        {
            if (player.GetModPlayer<SetBonusPlayer>().pulsarUpgrade && Main.myPlayer == player.whoAmI)
            {
                int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().
                    ApplyTo(50 + (adept.Level * 0.25f) * (1 + (adept.Stage * 0.05f)));
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<WideThunder>(), finalDamage, 2, player.whoAmI, 0);
            }
            base.OnOverheatRecovery(player, adept);
        }

        public override void SetArmedPhenomenonEquip(Player player, SeptimaPlayer adept, Mod mod)
        {
            player.head = EquipLoader.GetEquipSlot(mod, "AzureStrikerArmedPhenomenon", EquipType.Head);
            player.body = EquipLoader.GetEquipSlot(mod, "AzureStrikerArmedPhenomenon", EquipType.Body);
            player.legs = EquipLoader.GetEquipSlot(mod, "AzureStrikerArmedPhenomenon", EquipType.Legs);
            player.handon = EquipLoader.GetEquipSlot(mod, "AzureStrikerArmedPhenomenon", EquipType.HandsOn);
            player.handoff = EquipLoader.GetEquipSlot(mod, "AzureStrikerArmedPhenomenon", 
                EquipType.HandsOff);
            player.waist = EquipLoader.GetEquipSlot(mod, "AzureStrikerArmedPhenomenon", EquipType.Waist);
            player.wings = EquipLoader.GetEquipSlot(mod, "AzureStrikerArmedPhenomenon", EquipType.Wings);
        }

        public override void ArmedPhenomenonPreUpdate(Player player, SeptimaPlayer adept, int potency)
        {
            if (player.ItemAnimationActive && Main.myPlayer == player.whoAmI)
            {
                Item item = player.HeldItem;
                if (item.axe <= 0 && item.pick <= 0 && item.hammer <= 0 && item.createTile == -1 &&
                    item.createWall == -1 && item.damage > 0 && !item.accessory && item.defense <= 0 &&
                    !item.vanity)
                {
                    if (ArmedPhenomenonClawCooldown <= 0)
                    {
                        float baseClawDamage = 20 + (potency * 10) + (adept.Stage * 5);
                        int finalDamage = (int)player.GetTotalDamage<MainAttackDamage>().
                            ApplyTo(baseClawDamage);
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, 
                            player.Center.DirectionTo(Main.MouseWorld), 
                            ModContent.ProjectileType<AzureStrikerClaw>(), finalDamage, 3, 
                            player.whoAmI);
                        ArmedPhenomenonClawCooldown = 85 - (5 * potency);
                    }
                }
            }
        }

        public override void ArmedPhenomenonPostEquipUpdate(Player player, SeptimaPlayer adept, int potency)
        {
            if (!adept.Overheated)
            {
                player.wingTimeMax += 20 + (20 * potency);
            }

            player.noFallDmg = true;
            if (!adept.Overheated)
            {
                player.statDefense += 5 * potency;
                player.endurance += 0.06f * (potency - 1);
            }

            adept.EPUseModifier += (0.15f * potency);
            adept.EPRecoveryModifier += (0.2f * potency);
            adept.EPCooldownModifier += (0.2f * potency);
            adept.OverheatRecoveryModifier -= 0.25f;
            player.GetDamage<SeptimaDamage>() += 0.1f + (0.05f * potency);

            if (potency >= 3)
            {
                adept.SPRecoveryModifier += 0.2f;
            }
        }

        public override float GetBasicSkillPower(Player player, SeptimaPlayer adept)
        {
            float returnValue = 1;
            if (adept.DnizerMode)
            {
                returnValue += BaseBasicAttackDamage + (adept.Stage * 2) + (adept.Level * 0.25f);
            }
            if (player.GetModPlayer<PlayerBuffs>().DilationReticles)
            {
                if (adept.DnizerMode)
                {
                    returnValue *= 1.25f;
                } else
                {
                    float stageDamage = adept.Stage * 1.75f;
                    float levelDamage = adept.Level * 0.2f;
                    returnValue = BaseBasicAttackDamage + stageDamage + levelDamage;
                }
            }
            
            return returnValue;
        }

        public override float GetTagSkillPower(Player player, SeptimaPlayer adept, Tag tag, int tagCount = 1)
        {
            float returnValue = (BaseBasicAttackDamage + (adept.Level * 0.08f) + (adept.Stage)) * 
                (1f + ((float)tag.tagLevel * 0.5f)) / (1f + ((float)tagCount * 0.02f));
            if (player.GetModPlayer<PlayerBuffs>().DilationReticles)
            {
                returnValue *= 0.5f;
            }
            return returnValue;
        }

        public override float GetSecondarySkillPower(Player player, SeptimaPlayer adept)
        {
            float returnValue = 10 + (adept.Stage * 3) + (adept.Level * 0.4f);
            return returnValue;
        }

        public override bool GetSuperState(Player player, SeptimaPlayer adept)
        {
            ResurrectionPlayer resurrection = player.GetModPlayer<ResurrectionPlayer>();
            PlayerBuffs buffs = player.GetModPlayer<PlayerBuffs>();

            bool gv2SuperCheck = player.HasBuff<SeptimalSurgeBuff>() && resurrection.resurrectionPower >= 2;
            bool gv3SuperCheck = adept.DnizerMode;

            return (gv2SuperCheck || gv3SuperCheck) && !adept.Overheated;
        }

        public override void ResetEffects(Player player, SeptimaPlayer adept)
        {
            if (ArmedPhenomenonClawCooldown > 0)
            {
                ArmedPhenomenonClawCooldown--;
            }

            attackTimer++;
            if (attackTimer >= 2)
            {
                attackFrame += Main.rand.Next(-3, 3);
                attackTimer = 0;
                if (attackFrame > 6)
                {
                    attackFrame = 0;
                }
                if (attackFrame < 0)
                {
                    attackFrame = 6;
                }
            }

            attackRotation -= 0.001745329252f;
            sphereBaseRotation += MathHelper.TwoPi / 100;

            if (pulsarBonusTimer > 0) pulsarBonusTimer--;
        }
    }
}
