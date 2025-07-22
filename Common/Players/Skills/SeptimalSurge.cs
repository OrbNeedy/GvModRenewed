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
    public class SeptimalSurge : SpecialSkill
    {
        public override string InternalName { get; set; } = "SeptimalSurge";
        public override string LocalizationKey { get; set; } = "SeptimalSurge";
        public override bool Invincible { get; set; } = false;
        public override int LevelRequirement { get; set; } = 70;
        public override int StageRequirement { get; set; } = 5;
        public override int SPCost { get; set; } = 2;
        public override int MaxCooldownTime { get; set; } = 600;

        public override void MoveUpdate(Player player, SeptimaPlayer adept)
        {
            base.MoveUpdate(player, adept);
        }

        public override void StatUpdate(Player player, SeptimaPlayer adept)
        {
            base.StatUpdate(player, adept);
        }

        public override bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            player.AddBuff(ModContent.BuffType<SeptimalSurgeBuff>(), 1800);

            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(player.Center, DustID.Clentaminator_Red);
            }

            return true;
        }

        public override bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return adept.SpecialSkillUseTime < 60;
        }

        // If the player is in very specific circumstances, the use of Septimal Surge will mostly cancel attacks and
        // knockback
        public override void NPCHitUpdate(Player player, SeptimaPlayer adept, NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (player.GetModPlayer<ResurrectionPlayer>().resurrectionTime >= 36000 && 
                player.GetModPlayer<ResurrectionPlayer>().resurrected)
            {
                modifiers.SourceDamage.Base = 1;
                modifiers.Knockback.Base = 0;
            }
            base.NPCHitUpdate(player, adept, npc, ref modifiers);
        }

        public override void ProjectileHitUpdate(Player player, SeptimaPlayer adept, Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            if (player.GetModPlayer<ResurrectionPlayer>().resurrectionTime >= 36000 &&
                player.GetModPlayer<ResurrectionPlayer>().resurrected)
            {
                modifiers.SourceDamage.Base = 1;
                modifiers.Knockback.Base = 0;
            }
            base.ProjectileHitUpdate(player, adept, projectile, ref modifiers);
        }
    }
}
