using System.Collections.Generic;
using GvMod.Common.GlobalNPCs;
using GvMod.Common.Players.Skills;
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

        public override int BasicAttackDamage { get; protected set; } = 12;
        public override int SecondaryAttackDamage { get; protected set; } = 30;
        public override List<SpecialSkill> SkillList { get; protected set; } = new() { new SpecialSkill(), 
            new Astrasphere(), new GalvanicPatch() };
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

        public override void MiscEffects(Player player, SeptimaPlayer adept)
        {
            //player.GetDamage<SecondaryAttackDamage>() += 2;
            //player.GetArmorPenetration<SecondaryAttackDamage>() += 1000;
            //Main.NewText("Modifying defense");
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

            if ((!activeFlashfield || flashfieldIndex == -1) && Main.myPlayer == player.whoAmI)
            {
                flashfieldIndex = Projectile.NewProjectile(player.GetSource_Misc("Septima"), player.Center,
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

            float knockback = 0;
            if (adept.MainSkillUseTime <= 0)
            {
                adept.CurrentEP -= adept.GetTotalMaxEP() * 0.075f;
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
                float finalKnockback = player.GetTotalKnockback<MainAttackDamage>().ApplyTo(knockback);
                int direction = 1;
                if ((target.Center.X - player.Center.X) < 0)
                {
                    direction = -1;
                }

                player.ApplyDamageToNPC(target, finalDamage, knockback, direction, 
                    damageType: ModContent.GetInstance<MainAttackDamage>(), damageVariation: true);

                adept.TaggedNPCs.damageTimer[i] = 10;
            }

            return true;
        }

        public override int SecondarySkillUse(Player player, SeptimaPlayer adept)
        {
            if (adept.SecondarySkillUseTime == 0 && Main.myPlayer == player.whoAmI)
            {
                int finalDamage = (int)player.GetTotalDamage<SecondaryAttackDamage>().
                    ApplyTo(40);
                Projectile.NewProjectile(player.GetSource_Misc("Septima"), player.Center, Vector2.Zero, 
                    ModContent.ProjectileType<Thunder>(), finalDamage, 0, player.whoAmI, 1);
            }
            return adept.SecondarySkillUseTime >= 60 ? 600 : 0;
        }
    }
}
