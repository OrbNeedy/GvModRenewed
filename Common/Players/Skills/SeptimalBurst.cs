using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Content.Buffs;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace GvMod.Common.Players.Skills
{
    public class SeptimalBurst : SpecialSkill
    {
        public override string InternalName { get; set; } = "SeptimalBurst";
        public override string LocalizationKey { get; set; } = "SeptimalBurst";
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 27;
        public override int StageRequirement { get; set; } = 1;
        public override int APCost { get; set; } = 1;
        public override int MaxCooldownTime { get; set; } = 600;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            player.AddBuff(ModContent.BuffType<SeptimalBurstBuff>(), 1800);

            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(player.Center, DustID.Clentaminator_Purple);
            }

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 60;
        }
    }
}
