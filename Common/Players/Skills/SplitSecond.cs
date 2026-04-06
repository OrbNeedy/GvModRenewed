using Terraria;
using Terraria.ID;

namespace GvMod.Common.Players.Skills
{
    public class SplitSecond : SpecialSkill
    {
        public override string InternalName { get; set; } = "SplitSecond";
        public override string LocalizationKey { get; set; } = "SplitSecond";
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 27;
        public override int StageRequirement { get; set; } = 1;
        public override int SPCost { get; set; } = 1;
        public override int MaxCooldownTime { get; set; } = 1200;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(player.Center, DustID.Clentaminator_Cyan);
            }

            // Clear chaffed 
            //player.ClearBuff(ModContent.BuffType<>());

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            if (adept.CurrentEP < adept.GetTotalMaxEP())
            {
                adept.CurrentEP += adept.GetTotalMaxEP() / 60;
            }

            adept.Overheated = false;

            return adept.SpecialSkillUseTime < 60;
        }
    }
}
