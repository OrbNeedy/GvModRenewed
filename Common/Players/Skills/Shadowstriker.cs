using GvMod.Content.Projectiles;
using GvMod.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace GvMod.Common.Players.Skills
{
    public class Shadowstriker : SpecialSkill
    {
        public override string InternalName { get; set; } = "Shadowstriker";
        public override string LocalizationKey { get; set; } = "Shadowstriker";
        public override bool AllowsMovement { get; set; } = true;
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 0;
        public override int StageRequirement { get; set; } = 0;
        public override int SPCost { get; set; } = 0;
        public override int MaxCooldownTime { get; set; } = 1800;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            int baseDamage = 9 + (adept.Level);
            int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().ApplyTo(baseDamage);

            Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<ShadowstrikerProjectile>(), finalDamage, 0, player.whoAmI, -1); 
            Projectile.NewProjectile(player.GetSource_FromThis("Septima"), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<ShadowstrikerProjectile>(), finalDamage, 0, player.whoAmI, 1);

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 30;
        }

        public override bool? CustomUnlockCondition(Player player, SeptimaPlayer adept)
        {
            return adept.DragonVeinsVisited[2];
        }
    }
}
