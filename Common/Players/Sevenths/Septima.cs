using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GvMod.Common.Players.Skills;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

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
        public virtual int BasicAttackDamage { get; protected set; } = 0;
        public virtual int SecondaryAttackDamage { get; protected set; } = 0;
        public virtual List<SpecialSkill> SkillList { get; protected set; } = new() { new SpecialSkill() };
        public virtual List<SpecialSkill> AvailableSkills { get; protected set; } = new();
        public virtual float EPUseBase { get; protected set; } = 0;
        public virtual float EPRecoveryBaseRate { get; protected set; } = 0; 
        public virtual int EPCooldownBaseTimer { get; protected set; } = 0;
        public virtual float OverheatRecoveryBaseRate { get; protected set; } = 0;
        public virtual float APRecoveryBaseRate { get; protected set; } = 0f;

        // Identifiers
        public virtual SeptimaType Type { get; protected set; } = SeptimaType.None;
        public virtual string InternalName { get; private set; } = "None";
        public virtual Color MainColor { get; private set; } = Color.White;
        // Two separate colors so any septima can have distinct overheat and normal EP bar colors
        // This was basically made for Metallica and Energy Wool
        public virtual Color OverheatColor { get; private set; } = Color.DarkRed;

        // Modifiers
        public virtual int MaxEPModifier { get; set; } = 0;
        public virtual float EPRecoveryModifier { get; set; } = 0;
        public virtual float EPCooldownModifier { get; set; } = 0;
        public virtual float APRecoveryModifier { get; set; } = 0;
        public virtual float APCooldownModifier { get; set; } = 0;

        public virtual Dictionary<int, Resistance> NPCDamageResistances { get; set; } = new();
        public virtual Dictionary<int, Resistance> ProjectileDamageResistances { get; set; } = new();

        public void CalculateSkills(Player player, SeptimaPlayer adept)
        {
            AvailableSkills.AddRange(SkillList.FindAll((skill) =>
            {
                // Add all skills under the level and stage requirements that are also not included already
                return skill.LevelRequirement <= adept.Level && skill.StageRequirement <= adept.Stage && 
                    !AvailableSkills.Contains(skill);
            }));
        }

        public virtual void InitializeSeptima(Player player, SeptimaPlayer adept)
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

        public void UpdateTimers()
        {
            // For balance purposes, cooldown time reduction will not be modified regardless of septima or items
            foreach (SpecialSkill skill in AvailableSkills)
            {
                if (skill.CooldownTime > 0)
                {
                    skill.CooldownTime--;
                }
            }
        }
    }
}
