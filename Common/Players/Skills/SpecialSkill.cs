using Terraria;

namespace GvMod.Common.Players.Skills
{
    public class SpecialSkill
    {
        public virtual string InternalName { get; set; } = "Default";
        public virtual string LocalizationKey { get; set; } = "Default";
        public virtual int LevelRequirement { get; set; } = 0;
        public virtual int StageRequirement { get; set; } = 0;
        public virtual int APCost { get; set; } = 0;
        public virtual bool Invincible { get; set; } = false;
        public virtual int MaxCooldownTime { get; set; } = 0;
        public int CooldownTime { get; set; } = 0;
        /// <summary>
        /// Used by the <see cref="SeptimaPlayer"/> to determine if movement is allowed while using this special skill. <br/>
        /// It doesn't stop the player's innertia, if that is something the skill does, it must be done in <see cref="MoveUpdate(Player, SeptimaPlayer)"/>. <br/>
        /// Note: <see cref="Player.noFallDmg"/> is essential for skills that stop the player, if not true, the player will die of fall damage even if it was activated mid air.
        /// </summary>
        public virtual bool AllowsMovement { get; set; } = true;

        /// <summary>
        /// Runs whenever the skill is being set up, useful to make small differences between septimas with the same 
        /// skills, <br/>such as changing it's name or cost.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        public virtual void OnSetup(Player player, SeptimaPlayer adept)
        {

        }

        /// <summary>
        /// Determines if the special can be used at any moment.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns></returns>
        public virtual bool CanUse(Player player, SeptimaPlayer adept)
        {
            return true;
        }

        /// <summary>
        /// Used only the frame when the skill is being used, runs after <seealso cref="CanUse(Player, SeptimaPlayer)"/> returns true.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns>False to stop the skill from continuing.</returns>
        public virtual bool OnSkillUse(Player player, SeptimaPlayer adept)
        {
            return false;
        }

        /// <summary>
        /// Allows the special skill to modify player stats.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        public virtual void StatUpdate(Player player, SeptimaPlayer adept)
        {

        }

        /// <summary>
        /// Runs for as long as the skill needs to, useful to determine the duration of the skill based on something 
        /// <br/>other than time.<br/>Use <see cref="MiscUpdate(Player, SeptimaPlayer)"/> instead to modify player 
        /// stats.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns>False to stop the skill from continuing.</returns>
        public virtual bool MiscUpdate(Player player, SeptimaPlayer adept)
        {
            return false;
        }

        public virtual void MoveUpdate(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void NPCHitUpdate(Player player, SeptimaPlayer adept, NPC npc, 
            ref Player.HurtModifiers modifiers)
        {
        }

        public virtual void ProjectileHitUpdate(Player player, SeptimaPlayer adept, Projectile projectile,
            ref Player.HurtModifiers modifiers)
        {
        }

        public virtual void HurtUpdate(Player player, SeptimaPlayer adept, Player.HurtInfo info)
        {

        }

        /// <summary>
        /// Happens any time the skill is forced to end early, used for special interactions and stopping processes.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        public virtual void ForcedSkillEnd(Player player, SeptimaPlayer adept)
        {
        }

        /// <summary>
        /// A custom check for skills that unlock only if certain conditions are met.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns>True to unlock the skill, this will count even if the level and stage requirements are false.
        /// <br/>Return null to ignore this.</returns>
        public virtual bool? CustomUnlockCondition(Player player, SeptimaPlayer adept)
        {
            return null;
        }

        /// <summary>
        /// Similar to <see cref="CustomUnlockCondition(Player, SeptimaPlayer)"/>, it's a condition checked to 
        /// unlock a skill.<br/>
        /// Unlike it, the condition needs to be true in order to unlock the skill, even if stage, level or the 
        /// other custom <br/>condition are true.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns></returns>
        public virtual bool ForcedUnlockCondition(Player player, SeptimaPlayer adept)
        {
            return true;
        }
    }
}
