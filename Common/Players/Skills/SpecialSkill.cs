using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace GvMod.Common.Players.Skills
{
    public class SpecialSkill
    {
        public virtual string InternalName { get; set; } = "Default";
        public virtual string LocalizationKey { get; set; } = "Default";
        public virtual int LevelRequirement { get; set; } = 0;
        public virtual int StageRequirement { get; set; } = 0;
        public virtual List<Condition> AlternateUnlockConditions { get; set; } = new();
        public virtual List<Condition> UnlockConditions { get; set; } = new();
        public virtual int SPCost { get; set; } = 0;
        public virtual bool Invincible { get; set; } = false;
        public virtual int MaxCooldownTime { get; set; } = 0;
        public int CooldownTime { get; set; } = 0;
        /// <summary>
        /// Used by the <see cref="SeptimaPlayer"/> to determine if movement is allowed while using this special skill. <br/>
        /// It doesn't stop the player's innertia, if that is something the skill does, it must be done in <see cref="MoveUpdate(Player, SeptimaPlayer)"/>. <br/>
        /// Note: <see cref="Player.noFallDmg"/> is essential for skills that stop the player, if not true, the player will die of fall damage even if it was activated mid air.
        /// </summary>
        public virtual bool AllowsMovement { get; set; } = true;

        public virtual SpecialSkill SetLevel(int level)
        {
            LevelRequirement = level;
            return this;
        }

        public virtual SpecialSkill SetStage(int stage)
        {
            StageRequirement = stage;
            return this;
        }

        public virtual SpecialSkill SetUnlockConditions(params Condition[] conditions)
        {
            UnlockConditions.Clear();
            foreach (Condition condition in conditions)
            {
                UnlockConditions.Add(condition);
            }
            return this;
        }

        public virtual SpecialSkill SetAlternativeConditions(List<Condition> conditions)
        {
            AlternateUnlockConditions = conditions;
            return this;
        }

        public virtual SpecialSkill SetLocalization(string key)
        {
            LocalizationKey = key;
            return this;
        }

        public virtual SpecialSkill SetIconNames(string name)
        {
            InternalName = name;
            return this;
        }

        /// <summary>
        /// Allows overriding the name of the skill even if it's not being used.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns>Null to proceed with regular translation.</returns>
        public virtual string? GetFinalName(Player player, SeptimaPlayer adept)
        {
            return null;
        }

        /// <summary>
        /// Allows overriding the key for the name of the skill even if it's not being used.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns>Null to proceed with regular translation.</returns>
        public virtual string GetNewNameKey(Player player, SeptimaPlayer adept)
        {
            return LocalizationKey;
        }

        /// <summary>
        /// Allows overriding the description of the skill even if it's not being used.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns>Null to proceed with regular translation.</returns>
        public virtual string? GetFinalDescription(Player player, SeptimaPlayer adept)
        {
            return null;
        }

        /// <summary>
        /// Allows overriding the description of the skill even if it's not being used.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <returns>Null to proceed with regular translation.</returns>
        public virtual string GetNewDescriptionKey(Player player, SeptimaPlayer adept)
        {
            return LocalizationKey;
        }

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
        /// Used only the frame when the skill is activated, runs after <seealso cref="CanUse(Player, SeptimaPlayer)"/> returns true.
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

        // Because so many skills do this exact thing
        public void KeepPlayerInPlace(Player player)
        {
            player.noFallDmg = true;
            player.velocity = new Vector2(0, 0.0000001f);
            player.position = player.oldPosition;
            player.fallStart = (int)player.Center.Y;
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

        public virtual void NPCHurtUpdate(Player player, SeptimaPlayer adept, NPC npc, Player.HurtInfo info)
        {

        }

        public virtual void ProjectileHurtUpdate(Player player, SeptimaPlayer adept, Projectile proj, 
            Player.HurtInfo info)
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
    }
}
