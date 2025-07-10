using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Content.Buffs;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Skills
{
    public class SeptimalShield : SpecialSkill
    {
        public override string InternalName { get; set; } = "SeptimalShield";
        public override string LocalizationKey { get; set; } = "SeptimalShield";
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
            player.AddBuff(ModContent.BuffType<SeptimalShieldBuff>(), 1800);

            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(player.Center, DustID.Clentaminator_Blue);
            }

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 60;
        }
    }
}
