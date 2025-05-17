using Microsoft.Xna.Framework;
using Terraria;

namespace GvMod.Common.Players.Skills
{
    public class Astrasphere : SpecialSkill
    {
        public override string InternalName { get; set; } = "Astrasphere";
        public override bool Invincible { get; set; } = true;
        public override int LevelRequirement { get; set; } = 1;
        public override int StageRequirement { get; set; } = 1;
        public override int APCost { get; set; } = 1;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            player.noFallDmg = true;
            player.velocity = Vector2.Zero;
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            // Spawn
            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 120;
        }
    }
}
