using Terraria.ID;
using Terraria;

namespace GvMod.Common.Players.Skills
{
    public class GalvanicRenewal : SpecialSkill
    {
        public override string InternalName { get; set; } = "GalvanicRenewal";
        public override string LocalizationKey { get; set; } = "GalvanicRenewal";
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 1;
        public override int StageRequirement { get; set; } = 4;
        public override int SPCost { get; set; } = 2;
        public override int MaxCooldownTime { get; set; } = 3600;

        private int healPool = 0;
        private int healRate = 0;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            healPool = player.statLifeMax2;
            healRate = healPool / 60;
            if (healRate <= 0)
            {
                healRate = 1;
            }

            for (int i = 0; i < 50; i++)
            {
                if (i < 30)
                {
                    Dust.NewDustPerfect(player.Center, DustID.WhiteTorch);
                } else
                {
                    Dust.NewDustPerfect(player.Center, DustID.Clentaminator_Green);
                }
            }

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            // Stop healing if the heal pool runs out
            if (healPool > 0)
            {
                player.Heal(healRate);
                healPool -= healRate;
            }

            return adept.SpecialSkillUseTime < 60;
        }
    }
}
