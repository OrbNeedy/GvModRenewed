using Terraria;

namespace GvMod.Common.Players.Skills
{
    public class SpecialSkill
    {
        public virtual string InternalName { get; set; } = "Default";
        public virtual int LevelRequirement { get; set; } = 0;
        public virtual int StageRequirement { get; set; } = 0;
        public virtual int APCost { get; set; } = 0;
        public virtual bool Invincible { get; set; } = false;
        public int MaxCooldownTime { get; set; } = 0;
        public int CooldownTime { get; set; } = 0;
        /// <summary>
        /// Used by the <see cref="SeptimaPlayer"/> to determine if movement is allowed while using this special skill. 
        /// It doesn't stop the player's innertia, if that is something the skill does, it must be done in <see cref="MoveUpdate(Player, SeptimaPlayer)"/>. 
        /// Note: <see cref="Player.noFallDmg"/> is essential for skills that stop the player, if not true, the player will die of fall damage even if 
        /// it was activated mid air.
        /// </summary>
        public virtual bool AllowsMovement { get; set; } = false;

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
        /// Runs for as long as the skill needs to.
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

        public virtual bool NPCHitUpdate(Player player, SeptimaPlayer adept, NPC npc, 
            ref Player.HurtModifiers modifiers)
        {
            return true;
        }

        public virtual bool ProjectileHitUpdate(Player player, SeptimaPlayer adept, Projectile projectile,
            ref Player.HurtModifiers modifiers)
        {
            return true;
        }
    }
}
