using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace GvMod.Common.Players.Skills
{
    public class GalvanicPatch : SpecialSkill
    {
        public override string InternalName { get; set; } = "GalvanicPatch";
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 3;
        public override int StageRequirement { get; set; } = 1;
        public override int APCost { get; set; } = 1;
        public override int MaxCooldownTime { get; set; } = 1200;

        private int healPool = 0;
        private int healRate = 0;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            // Only heal 25% of the player's health divided in a second
            healPool = (int)(player.statLifeMax2 * 0.25f);
            healRate = healPool/60;
            if (healRate <= 0)
            {
                healRate = 1;
            }

            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(player.Center, DustID.Clentaminator_Green);
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
