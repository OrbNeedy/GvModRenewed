using GvMod.Content;
using GvMod.Content.Buffs;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Skills
{
    public class Luxcalibur : SpecialSkill
    {
        public override string InternalName { get; set; } = "Luxcalibur";
        public override string LocalizationKey { get; set; } = "Luxcalibur";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 20;
        public override int StageRequirement { get; set; } = 1;
        public override int SPCost { get; set; } = 2;
        public override int MaxCooldownTime { get; set; } = 720;

        public float initialPower = 0;

        public override string GetNewNameKey(Player player, SeptimaPlayer adept)
        {
            ResurrectionPlayer resurrectionState = player.GetModPlayer<ResurrectionPlayer>();
            if (resurrectionState.resurrected && resurrectionState.resurrectionPower >= 2)
            {
                return "LuxcaliburLauncher";
            }
            return base.GetNewNameKey(player, adept);
        }

        public override string GetNewDescriptionKey(Player player, SeptimaPlayer adept)
        {
            ResurrectionPlayer resurrectionState = player.GetModPlayer<ResurrectionPlayer>();
            if (resurrectionState.resurrected && resurrectionState.resurrectionPower >= 2)
            {
                return "LuxcaliburLauncher";
            }
            return base.GetNewDescriptionKey(player, adept);
        }

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            KeepPlayerInPlace(player);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            float baseDamage = 260 + (10 * adept.Stage) + (0.65f * adept.Level);
            player.oldPosition = player.position;
            int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().ApplyTo(baseDamage);

            int swordBehavior = (int)LuxcaliburBehavior.Default;

            ResurrectionPlayer resurrectionState = player.GetModPlayer<ResurrectionPlayer>();
            if (resurrectionState.resurrected && resurrectionState.resurrectionPower >= 2)
            {
                swordBehavior = (int)LuxcaliburBehavior.Launch;
            }

            if (player.GetModPlayer<ResurrectionPlayer>().resurrected)
            {
                initialPower = player.GetModPlayer<ResurrectionPlayer>().resurrectionPower;
            }
            else
            {
                initialPower = 0;
            }

            Vector2 direction = player.Center.DirectionTo(Main.MouseWorld);
            Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, direction * 26, 
                ModContent.ProjectileType<LuxcaliburProjectile>(), finalDamage, 3.5f, player.whoAmI, 
                swordBehavior);

            if (resurrectionState.resurrected && (resurrectionState.resurrectionPower >= 3 || 
                player.HasBuff<SeptimalSurgeBuff>()))
            {
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, 
                    direction.RotatedBy(MathHelper.PiOver2) * 26,
                    ModContent.ProjectileType<LuxcaliburProjectile>(), finalDamage, 3.5f, player.whoAmI,
                    swordBehavior); 
                Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, 
                    direction.RotatedBy(-MathHelper.PiOver2) * 26, 
                    ModContent.ProjectileType<LuxcaliburProjectile>(), finalDamage, 3.5f, player.whoAmI, 
                    swordBehavior);
            }

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            if (initialPower >= 2) return adept.SpecialSkillUseTime < 130;

            return adept.SpecialSkillUseTime < 90;
        }

        public override void StatUpdate(Player player, SeptimaPlayer adept)
        {
            player.statDefense += 15;
            player.endurance += 0.15f;
            player.GetArmorPenetration<SpecialAttackDamage>() += 100f;
        }
    }
}
