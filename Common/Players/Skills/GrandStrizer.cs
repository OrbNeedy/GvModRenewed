using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Content;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Skills
{
    public class GrandStrizer : SpecialSkill
    {
        public override string InternalName { get; set; } = "GrandStrizer";
        public override string LocalizationKey { get; set; } = "GrandStrizer";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = true;
        public override int LevelRequirement { get; set; } = 72;
        public override int StageRequirement { get; set; } = 6;
        public override int APCost { get; set; } = 4;
        public override int MaxCooldownTime { get; set; } = 720;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            player.noFallDmg = true;
            player.velocity = Vector2.Zero;
            player.position = player.oldPosition;
            player.fallStart = (int)player.Center.Y;
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            float baseDamage = 300 + (5 * adept.Stage);
            baseDamage *= MathHelper.Clamp(player.statLife/player.statLifeMax2, 1, 2);
            player.oldPosition = player.position;
            int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().ApplyTo(baseDamage);

            Vector2 direction = player.Center.DirectionTo(Main.MouseWorld);
            Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, direction * 34,
                ModContent.ProjectileType<GrandStrizerProjectile>(), finalDamage, 5, player.whoAmI,
                (int)LuxcaliburBehavior.Default);

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 120;
        }

        public override void StatUpdate(Player player, SeptimaPlayer adept)
        {
            player.GetArmorPenetration<SpecialAttackDamage>() += 150f;
            player.noKnockback = true;
        }
    }
}
