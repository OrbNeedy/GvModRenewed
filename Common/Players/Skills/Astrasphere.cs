using Microsoft.Xna.Framework;
using Terraria;

namespace GvMod.Common.Players.Skills
{
    public class Astrasphere : SpecialSkill
    {
        public override string InternalName { get; set; } = "Astrasphere";
        public override bool AllowsMovement { get; set; } = false;
        public override bool Invincible { get; set; } = true;
        public override int LevelRequirement { get; set; } = 1;
        public override int StageRequirement { get; set; } = 1;
        public override int APCost { get; set; } = 1;
        public override int MaxCooldownTime { get; set; } = 600;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            player.noFallDmg = true;
            player.velocity = Vector2.Zero;
            player.position = player.oldPosition;
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            player.oldPosition = player.position;
            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 180;
        }
    }
}
