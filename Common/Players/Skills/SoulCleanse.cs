using Terraria.ID;
using Terraria;
using GvMod.Content.Buffs;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Skills
{
    public class SoulCleanse : SpecialSkill
    {
        public override string InternalName { get; set; } = "SoulCleanse";
        public override string LocalizationKey { get; set; } = "SoulCleanse";
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 20;
        public override int StageRequirement { get; set; } = 2;
        public override int SPCost { get; set; } = 1;
        public override int MaxCooldownTime { get; set; } = 1200;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            player.AddBuff(ModContent.BuffType<SoulCleanseBuff>(), 2700);

            int maxDebuffClears = 3;
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (maxDebuffClears <= 0) break;

                // Debuff and can be cleared by the nurse
                if (Main.debuff[player.buffType[i]] &&
                    !BuffID.Sets.NurseCannotRemoveDebuff[player.buffType[i]] &&
                    Main.buffNoSave[player.buffType[i]])
                {
                    player.DelBuff(i);
                    i--;
                    maxDebuffClears--;
                }
            }

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
