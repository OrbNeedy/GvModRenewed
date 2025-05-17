﻿using GvMod.Common.GlobalNPCs;
using GvMod.Content;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Sevenths
{
    public class AzureStriker : Septima
    {
        // Septima uniques
        public bool activeFlashfield = false;
        public int flashfieldIndex = -1;

        public override int BasicAttackDamage { get; protected set; } = 8;
        public override int SecondaryAttackDamage { get; protected set; } = 30;
        public override float EPUseBase { get; protected set; } = 0.45f;
        public override float EPRecoveryBaseRate { get; protected set; } = 0.006666f;
        public override int EPCooldownBaseTimer { get; protected set; } = 90;
        public override float OverheatRecoveryBaseRate { get; protected set; } = 0.003333f;
        public override float APRecoveryBaseRate { get; protected set; } = 0.000185f;

        public override SeptimaType Type { get; protected set; } = SeptimaType.AzureStriker;
        public override string InternalName => "AzureStriker";
        public override Color MainColor => new Color(77, 242, 229);

        public override void InitializeSeptima(Player player, SeptimaPlayer adept)
        {
            NPCDamageResistances =  new() {
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
                [ProjectileID.CultistBossLightningOrbArc] = Resistance.Absorb,
                [ModContent.ProjectileType<Flashfield>()] = Resistance.Penetrate
            };
        }

        public override void MovementEffects(Player player, SeptimaPlayer adept)
        {
            player.maxRunSpeed *= 1.075f;
            player.runAcceleration *= 1.2f;
        }

        public override bool MainSkillUse(Player player, SeptimaPlayer adept)
        {
            if (player.wet)
            {
                adept.ForceOverheat();
                return true;
            }

            if (!activeFlashfield)
            {
                flashfieldIndex = Projectile.NewProjectile(player.GetSource_Misc("Seventh"), player.Center,
                    Vector2.Zero, ModContent.ProjectileType<Flashfield>(), 1, 0, player.whoAmI,
                    (int)FlashfieldBehavior.Default);
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
            player.maxFallSpeed *= 0.2f;
            if (adept.MainSkillUseTime <= 0) adept.CurrentEP -= EPUseBase * adept.GetTotalEPUseModifier() * 19;

            //Main.NewText("Main Skill: " + adept.MainSkillUseTime);
            // Deal damage to tagged NPCs
            for (int i = 0; i < adept.TaggedNPCs.targetCount; i++)
            {
                // Tell the taggedNPC to show damage effects
                NPC target = Main.npc[adept.TaggedNPCs.taggedTargets[i]];
                target.GetGlobalNPC<TagNPC>().attacked = true;
                target.GetGlobalNPC<TagNPC>().framesUntilReset = 2;

                if (adept.TaggedNPCs.damageTimer[i] > 0) continue;

                float knockback = 0;
                if (adept.MainSkillUseTime <= 0) knockback = 2.5f;
                int finalDamage = (int)player.GetTotalDamage<MainAttackDamage>().
                    ApplyTo(BasicAttackDamage * (1 + (adept.TaggedNPCs.tagLevel[i] * 0.625f)));
                float finalKnockback = player.GetTotalKnockback<MainAttackDamage>().ApplyTo(knockback);
                int direction = 1;
                if ((target.Center.X - player.Center.X) < 0)
                {
                    direction = -1;
                }

                NPC.HitInfo info = target.CalculateHitInfo(20, direction,
                    false, knockback, ModContent.GetInstance<MainAttackDamage>(), true);

                player.ApplyDamageToNPC(target, finalDamage, knockback, direction, 
                    damageType: ModContent.GetInstance<MainAttackDamage>(), damageVariation: true);

                adept.TaggedNPCs.damageTimer[i] = 10;
            }

            return true;
        }
    }
}
