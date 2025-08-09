using System.Collections;
using System.Collections.Generic;
using GvMod.Common.Players.Skills;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GvMod.Common.Players.Sevenths
{
    public enum Resistance
    {
        None, 
        Penetrate, 
        Overheat, 
        Ignore, 
        Absorb
    }

    public enum SeptimaType
    {
        None, 
        AzureStriker
    }
    public class Septima
    {
        // Base values
        public virtual float BaseBasicAttackDamage { get; protected set; } = 0;
        public virtual float BasicAttackDamage { get; protected set; } = 0;
        public virtual float BaseSecondaryAttackDamage { get; protected set; } = 0;
        public virtual float SecondaryAttackDamage { get; protected set; } = 0;
        public virtual List<SpecialSkill> SkillList { get; protected set; } = new() { new SpecialSkill() };
        public virtual List<SpecialSkill> AvailableSkills { get; protected set; } = new();
        public virtual float EPUseBase { get; protected set; } = 0;
        public virtual float EPRecoveryBaseRate { get; protected set; } = 0; 
        public virtual int EPCooldownBaseTimer { get; protected set; } = 0;
        public virtual float OverheatRecoveryBaseRate { get; protected set; } = 0;
        public virtual float SPRecoveryBaseRate { get; protected set; } = 0f;
        public virtual bool AllowRecharge { get; protected set; } = true;
        public virtual bool AllowPrevasion { get; protected set; } = true;
        public virtual int PrevasionEPCooldownBaseTimer { get; protected set; } = 0;

        // Identifiers
        public virtual SeptimaType Type { get; protected set; } = SeptimaType.None;
        public virtual string InternalName { get; private set; } = "None";
        public virtual Color MainColor { get; private set; } = Color.White;
        // Two separate colors so any septima can have distinct overheat and normal EP bar colors
        // This was basically made for septimas with red colors in their design
        public virtual Color OverheatColor { get; private set; } = Color.DarkRed;

        // Modifiers
        public virtual int MaxEPModifier { get; set; } = 0;
        public virtual float EPRecoveryModifier { get; set; } = 0;
        public virtual float EPCooldownModifier { get; set; } = 0;
        public virtual float SPRecoveryModifier { get; set; } = 0;
        public virtual float APCooldownModifier { get; set; } = 0;

        public virtual Dictionary<int, Resistance> NPCDamageResistances { get; set; } = new();
        public virtual Dictionary<int, Resistance> ProjectileDamageResistances { get; set; } = new();

        public void CalculateSkills(Player player, SeptimaPlayer adept, bool queue = false)
        {
            // TODO: Return the displacement of the index so the player's selected skill won't change after sorting
            List<SpecialSkill> SkillsToAdd = SkillList.FindAll((skill) =>
            {
                bool baseRequirements = skill.LevelRequirement <= adept.Level &&
                    skill.StageRequirement <= adept.Stage;
                bool forcedRequirement = skill.ForcedUnlockCondition(player, adept);
                bool? customCondition = skill.CustomUnlockCondition(player, adept);
                // Add all skills under the level and stage requirements that are also not included already
                if (customCondition == null)
                {
                    return baseRequirements && !AvailableSkills.Contains(skill) && forcedRequirement;
                } else
                {
                    return (bool)customCondition && !AvailableSkills.Contains(skill) && forcedRequirement;
                }
            });

            AvailableSkills.AddRange(SkillsToAdd);

            AvailableSkills.Sort(new SkillComparer(4));

            foreach (SpecialSkill skill in AvailableSkills)
            {
                skill.OnSetup(player, adept);
            }

            if (queue)
            {
                foreach (SpecialSkill skill in SkillsToAdd)
                {
                    adept.QueuedSkills.Add(skill.InternalName);
                }
            }
        }

        public virtual void LoadSeptima(Mod mod)
        {

        }

        public virtual void InitializeSeptima(Player player, SeptimaPlayer adept, Mod mod)
        {
        }

        public virtual void PostLoadSeptima(Player player, SeptimaPlayer adept)
        {
        }

        public virtual void MovementEffects(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void MiscEffects(Player player, SeptimaPlayer adept)
        {

        }

        public virtual bool CanUseMainSkill(Player player, SeptimaPlayer adept)
        {
            return true;
        }

        // Return value determines if the use spends any EP
        // Used with a septima timer to determine different use types
        public virtual bool MainSkillUse(Player player, SeptimaPlayer adept)
        {
            return true;
        }

        public virtual bool CanUseSecondarySkill(Player player, SeptimaPlayer adept)
        {
            return true;
        }

        public virtual int SecondarySkillUse(Player player, SeptimaPlayer adept)
        {
            return 0;
        }

        public virtual void OnOverheat(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void OnOverheatRecovery(Player player, SeptimaPlayer adept)
        {

        }

        public void UpdateTimers(bool perfectionFlag)
        {
            // For balance purposes, cooldown time reduction will not be modified regardless of septima or items
            foreach (SpecialSkill skill in AvailableSkills)
            {
                if (skill.CooldownTime > 0)
                {
                    if (perfectionFlag)
                    {
                        skill.CooldownTime -= 10;
                    } else
                    {
                        skill.CooldownTime--;
                    }
                }
            }
        }

        public void ForceCooldownEnd()
        {
            foreach (SpecialSkill skill in AvailableSkills)
            {
                skill.CooldownTime = 0;
            }
        }

        public virtual void OnLevelUp(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void OnStageChange(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void OnVeinVisit(Player player, SeptimaPlayer adept, int index)
        {

        }

        public virtual void DuringVeinVisit(Player player, SeptimaPlayer adept, int index, float distance)
        {

        }

        /// <summary>
        /// Runs after any enemy with the boss flag on is defeated, the boss' defeat flag is still not set here.
        /// </summary>
        /// <param name="bossID"></param>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        public virtual void OnBossDefeat(int bossID, Player player, SeptimaPlayer adept)
        {

        }

        public virtual void DrawPassive(ref PlayerDrawSet drawInfo, Player player, SeptimaPlayer adept)
        {

        }

        public virtual void DrawAttack(ref PlayerDrawSet drawInfo, Player player, SeptimaPlayer adept)
        {

        }

        public virtual void OnDnizerActive(Player player, SeptimaPlayer adept)
        {

        }

        public virtual void SetArmedPhenomenonEquip(Player player, SeptimaPlayer adept, Mod mod)
        {

        }

        public virtual void ArmedPhenomenonPreUpdate(Player player, SeptimaPlayer adept, int potency)
        {

        }

        public virtual void ArmedPhenomenonPostEquipUpdate(Player player, SeptimaPlayer adept, int potency)
        {

        }

        public virtual void ItemUse(Player player, SeptimaPlayer adept, Item item)
        {

        }
    }

    public class SkillComparer : IComparer<SpecialSkill>
    {
        float stageRelevance = 1;

        public SkillComparer(float stageRelevance)
        {
            this.stageRelevance = stageRelevance;
        }

        public int Compare(SpecialSkill x, SpecialSkill y)
        {
            float result = 0;

            result += (x.StageRequirement - y.StageRequirement) * stageRelevance;
            result += (x.LevelRequirement - y.StageRequirement);

            return (int)result;
        }
    }
}
