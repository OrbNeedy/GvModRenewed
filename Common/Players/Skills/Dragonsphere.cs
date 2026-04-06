using Terraria.ModLoader;
using Terraria;
using GvMod.Content;
using GvMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GvMod.Common.Players.Skills
{
    public class Dragonsphere : SpecialSkill
    {
        public override string InternalName { get; set; } = "Dragonsphere";
        public override string LocalizationKey { get; set; } = "Dragonsphere";
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
                ModContent.ProjectileType<DragonsphereProjectile>(), finalDamage, 2, player.whoAmI);

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 90;
        }
    }
}
