using Microsoft.Xna.Framework;
using Terraria;

namespace GvMod.Common.Players.Skills
{
    public class GalvanicPatch : SpecialSkill
    {
        public override string InternalName { get; set; } = "GalvanicPatch";
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 1;
        public override int StageRequirement { get; set; } = 1;
        public override int APCost { get; set; } = 1;
        public override int MaxCooldownTime { get; set; } = 900;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            Main.NewText("So much healing wow");
            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            player.Heal(1);
            return adept.SpecialSkillUseTime < 60;
        }
    }
}
