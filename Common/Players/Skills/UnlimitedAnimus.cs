using GvMod.Content.Buffs;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace GvMod.Common.Players.Skills
{
    class UnlimitedAnimus : SpecialSkill
    {
        public override string InternalName { get; set; } = "UnlimitedAnimus";
        public override string LocalizationKey { get; set; } = "UnlimitedAnimus";
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 80;
        public override int StageRequirement { get; set; } = 7;
        public override int SPCost { get; set; } = 0;
        public override int MaxCooldownTime { get; set; } = 5400;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            player.AddBuff(ModContent.BuffType<UnlimitedAnimusBuff>(), 1800);

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(player.Center, DustID.Clentaminator_Blue);
            }
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(player.Center, DustID.Clentaminator_Red);
            }

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 60;
        }
    }
}
