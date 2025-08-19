using GvMod.Content.Buffs;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace GvMod.Common.Players.Skills
{
    public class GFree : SpecialSkill
    {
        public override string InternalName { get; set; } = "GFree";
        public override string LocalizationKey { get; set; } = "GFree";
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 70;
        public override int StageRequirement { get; set; } = 6;
        public override int SPCost { get; set; } = 2;
        public override int MaxCooldownTime { get; set; } = 900;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            player.AddBuff(ModContent.BuffType<GFreeBuff>(), 1800);

            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(player.Center, DustID.Clentaminator_Cyan);
            }

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 60;
        }
    }
}
