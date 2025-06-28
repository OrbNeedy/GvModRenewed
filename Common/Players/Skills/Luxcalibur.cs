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
    public class Luxcalibur : SpecialSkill
    {
        public override string InternalName { get; set; } = "Luxcalibur";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 20;
        public override int StageRequirement { get; set; } = 1;
        public override int APCost { get; set; } = 2;
        public override int MaxCooldownTime { get; set; } = 720;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            player.noFallDmg = true;
            player.velocity = Vector2.Zero;
            player.position = player.oldPosition;
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            int baseDamage = 175;
            player.oldPosition = player.position;
            int finalDamage = (int)player.GetTotalDamage<SpecialAttackDamage>().ApplyTo(baseDamage);

            Vector2 direction = player.Center.DirectionTo(Main.MouseWorld);
            int projRef = Projectile.NewProjectile(player.GetSource_Misc("Septima"), player.Center, direction * 25, 
                ModContent.ProjectileType<LuxcaliburProjectile>(), finalDamage, 5, player.whoAmI, 
                (int)LuxcaliburBehavior.Default);
            Main.NewText("Projectile: " + Main.projectile[projRef].Name);

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 120;
        }

        public override void StatUpdate(Player player, SeptimaPlayer adept)
        {
            player.statDefense += 10;
            player.endurance += 0.25f;
        }
    }
}
