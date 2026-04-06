using Terraria.ID;
using Terraria;
using GvMod.Content.Buffs;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Skills
{
    public class AlchemicalField : SpecialSkill
    {
        public override string InternalName { get; set; } = "AlchemicalField";
        public override string LocalizationKey { get; set; } = "AlchemicalField";
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 65;
        public override int StageRequirement { get; set; } = 5;
        public override int SPCost { get; set; } = 2;
        public override int MaxCooldownTime { get; set; } = 600;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            player.AddBuff(ModContent.BuffType<AlchemicalFieldBuff>(), 1800);

            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(player.Center, DustID.SparksMech);
            }

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 60;
        }
    }
}
