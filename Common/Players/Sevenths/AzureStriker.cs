using System.Collections.Generic;
using GvMod.Common.GlobalNPCs;
using GvMod.Common.Players.Skills;
using GvMod.Content;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Sevenths
{
    public class AzureStriker : Septima
    {
        // Septima uniques
        public bool activeFlashfield = false;
        public int flashfieldIndex = -1;

        public int attackFrame = 0;
        public int attackTimer = 0;
        public float attackRotation = 0;

        public int ArmedPhenomenonClawCooldown = 0;

        public override float BaseBasicAttackDamage { get; protected set; } = 5;
        public override float BasicAttackDamage { get; protected set; } = 5;
        public override float BaseSecondaryAttackDamage { get; protected set; } = 20;
        public override float SecondaryAttackDamage { get; protected set; } = 20;
        public override List<SpecialSkill> SkillList { get; protected set; } = new() { new SpecialSkill(),
            new Astrasphere(), new GalvanicPatch(), new Luxcalibur(), new VoltaicChains(), new AlchemicalField(), 
            new InfiniteSurge(), new GalvanicRenewal(), new SeptimalBurst(), new SeptimalShield(), 
            new SeptimalSurge(), new SplitSecond(), new GrandStrizer(), new Dragonsphere()
        };
        public override float EPUseBase { get; protected set; } = 0.75f;
        public override float EPRecoveryBaseRate { get; protected set; } = 0.006666f;
        public override int EPCooldownBaseTimer { get; protected set; } = 90;
        public override float OverheatRecoveryBaseRate { get; protected set; } = 0.003333f;
        public override float SPRecoveryBaseRate { get; protected set; } = 0.000185f;
        public override int PrevasionEPCooldownBaseTimer { get; protected set; } = 90;

        public override SeptimaType Type { get; protected set; } = SeptimaType.AzureStriker;
        public override string InternalName => "AzureStriker";
        public override Color MainColor => new Color(77, 242, 229);

        public override void InitializeSeptima(Player player, SeptimaPlayer adept, Mod mod)
        {
            NPCDamageResistances = new() {
                [NPCID.WaterSphere] = Resistance.Penetrate,
                [NPCID.Sharkron] = Resistance.Penetrate,
                [NPCID.Sharkron2] = Resistance.Penetrate
            };

            ProjectileDamageResistances = new()
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
                [ModContent.ProjectileType<Flashfield>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<AstrasphereProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<AstrasphereOrbits>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<LuxcaliburProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<VoltaicChainProjectile>()] = Resistance.Penetrate,
                [ModContent.ProjectileType<GrandStrizerProjectile>()] = Resistance.Penetrate
            };

            int headID = EquipLoader.GetEquipSlot(mod, "AzureStrikerArmedPhenomenon", EquipType.Head);
            ArmorIDs.Head.Sets.DrawFullHair[headID] = true;
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

        public override void PostLoadSeptima(Player player, SeptimaPlayer adept)
        {
            // Increase by 0.1 every level
            // Sounds like little, but it's a lot with flashfield
            // BasicAttackDamage = BaseBasicAttackDamage + (adept.Level * 0.1f);
            base.PostLoadSeptima(player, adept);
        }

        public override void MiscEffects(Player player, SeptimaPlayer adept)
        {
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

            if (flashfieldIndex > -1)
            {
                Projectile flashfield = Main.projectile[flashfieldIndex];
                activeFlashfield = flashfield.active && flashfield.ModProjectile is Flashfield &&
                    flashfield.owner == player.whoAmI;
            } else
            {
                activeFlashfield = false;
            }

            AllowPrevasion = !activeFlashfield;

            BasicAttackDamage = BaseBasicAttackDamage + (adept.Level * 0.1f);

            if (ArmedPhenomenonClawCooldown > 0)
            {
                ArmedPhenomenonClawCooldown--;
            }
            // Main.NewText("Y pos: " + player.Center.Y);
        }

        public override void MovementEffects(Player player, SeptimaPlayer adept)
        {
            player.maxRunSpeed *= 1.075f;
            player.runAcceleration *= 1.2f;
        }

        public override bool MainSkillUse(Player player, SeptimaPlayer adept)
        {
            // Main.NewText("Frames: " + adept.MainSkillUseTime);
            if (player.wet)
            {
                if (adept.Stage <= 5)
                {
                    if (adept.ForceOverheat()) return true;
                } else if (adept.Stage <= 7)
                {
                    adept.EPUseModifier *= 2;
                }
            }

            if ((!activeFlashfield || flashfieldIndex == -1) && Main.myPlayer == player.whoAmI)
            {
                flashfieldIndex = Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center,
                    Vector2.Zero, ModContent.ProjectileType<Flashfield>(), 1, 0, player.whoAmI);
            }

            Projectile flashfield = Main.projectile[flashfieldIndex];
            activeFlashfield = flashfield.active && flashfield.ModProjectile is Flashfield &&
                flashfield.owner == player.whoAmI;

            // Reset timer and assert friendlyness
            if (activeFlashfield)
            {
                flashfield.timeLeft = 3;
                flashfield.friendly = true;
                flashfield.hostile = false;
                flashfield.netUpdate = true;
            }

            // Give player fall immunity
            player.noFallDmg = true;
            player.maxFallSpeed *= 0.25f;
            player.fallStart = (int)player.Center.Y;

            float knockback = 0;
            if (adept.MainSkillUseTime <= 0)
            {
                adept.CurrentEP -= adept.GetTotalMaxEP() * 0.08f * adept.GetTotalEPUseModifier();
                knockback = 2.5f;
            }

            //Main.NewText("Main Skill: " + adept.MainSkillUseTime);
            // Deal damage to tagged NPCs
            // TODO: Move this loop to the septima player with a method for the septima to use 
            for (int i = 0; i < adept.TaggedNPCs.targetCount; i++)
            {
                // Tell the taggedNPC to show damage effects
                NPC target = Main.npc[adept.TaggedNPCs.taggedTargets[i]];
                target.GetGlobalNPC<TagNPC>().attacked = true;
                target.GetGlobalNPC<TagNPC>().framesUntilReset = 2;

                if (adept.TaggedNPCs.damageTimer[i] > 0) continue;

                // Damage gets reduced if the player has too many tags
                float adjustedDamage = BasicAttackDamage * (1f + (adept.TaggedNPCs.tagLevel[i] * 0.625f))
                    / (1 + (adept.TaggedNPCs.targetCount * 0.075f));
                int finalDamage = (int)player.GetTotalDamage<MainAttackDamage>().
                    ApplyTo(adjustedDamage);
                int direction = 1;
                if ((target.Center.X - player.Center.X) < 0)
                {
                    direction = -1;
                }

                bool crit = player.GetTotalCritChance<SpecialAttackDamage>() < Main.rand.NextFloat();
                player.ApplyDamageToNPC(target, finalDamage, knockback, direction, crit, 
                    ModContent.GetInstance<MainAttackDamage>(), true);
                // player.Hurt(new PlayerDeathReason(), finalDamage, direction, true, dodgeable: false);

                adept.TaggedNPCs.damageTimer[i] = 10;
            }

            return true;
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
                    ApplyTo(38 + adept.Stage * 2);
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero, 
                    ModContent.ProjectileType<Thunder>(), finalDamage, 0, player.whoAmI, 1);
            }
            return adept.SecondarySkillUseTime >= 60 ? 600 : 0;
        }

        public override void OnLevelUp(Player player, SeptimaPlayer adept)
        {
            BasicAttackDamage = BaseBasicAttackDamage + (adept.Level * 0.1f) + ((int)(adept.Stage / 5) * 20);
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
                    if (i > 9) break;
                    NPC target = Main.npc[adept.TaggedNPCs.taggedTargets[i]];
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

                        Main.EntitySpriteDraw(
                            thunder.Value,
                            currentPosition - Main.screenPosition,
                            bounds,
                            Color.White * alpha,
                            rotationToNext,
                            new Vector2(0, 38),
                            1f,
                            SpriteEffects.None
                        );

                        if (currentPosition.Distance(target.Center) <= 128) break;

                        nextDirection.Normalize();

                        baseDirection = nextDirection;
                        currentPosition += baseDirection * 128f;
                    }
                }
            }
            base.DrawAttack(ref drawInfo, player, adept);
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

        public override void ItemUse(Player player, SeptimaPlayer adept, Item item)
        {
        }

        public override void ArmedPhenomenonPreUpdate(Player player, SeptimaPlayer adept, int potency)
        {
            if (player.ItemAnimationActive)
            {
                Item item = player.HeldItem;
                if (item.axe <= 0 && item.pick <= 0 && item.hammer <= 0 && item.createTile == -1 &&
                    item.createWall == -1 && item.damage > 0 && !item.accessory && item.defense <= 0 &&
                    !item.vanity)
                {
                    if (ArmedPhenomenonClawCooldown <= 0)
                    {
                        float baseClawDamage = 20 + (potency * 5) + (adept.Stage * 5);
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
            /*if (player.GetModPlayer<PlayerBuffs>().ArmedPhenomenonStats > 0)
            {
                if (item.axe > 0 || item.pick > 0 || item.hammer > 0 || item.createTile != -1 ||
                    item.createWall != -1 || item.damage <= 0 || item.accessory || item.defense > 0 ||
                    item.vanity) return;

                if (ArmedPhenomenonClawCooldown <= 0)
                {
                    ArmedPhenomenonClawCooldown = 75;
                }
            }*/
        }

        public override void ArmedPhenomenonPostEquipUpdate(Player player, SeptimaPlayer adept, int potency)
        {
            if (!adept.Overheated)
            {
                player.wingTimeMax += (45 * potency);
            }

            player.noFallDmg = true;
            if (!adept.Overheated)
            {
                player.statDefense += 5 * potency;
                player.endurance += 0.06f * (potency - 1);
            }

            adept.EPUseModifier *= 1.25f + (0.25f * potency);
            adept.EPRecoveryModifier *= 1f + (0.2f * potency);
            adept.OverheatRecoveryModifier *= 1f + (0.2f * potency);
            player.GetDamage<MainAttackDamage>() += 0.1f + (0.1f * potency);
            player.GetDamage<SecondaryAttackDamage>() += 0.1f + (0.1f * potency);

            if (potency >= 3)
            {
                adept.SPRecoveryModifier += 0.1f;
            }
        }
    }
}
