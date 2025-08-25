using System.Collections.Generic;
using GvMod.Common.Players.Skills;
using GvMod.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
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
        public virtual float BaseBasicAttackDamage { get; protected set; } = 0;
        public virtual float BaseSecondaryAttackDamage { get; protected set; } = 0;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="adept"></param>
        /// <param name="index"></param>
        /// <param name="tags"></param>
        /// <returns>The damage dealt by the tag attack.</returns>
        public virtual int TagEffect(Player player, SeptimaPlayer adept, int index, ref NPCTags tags)
        {
            return 0;
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

        public virtual float GetBasicSkillPower(Player player, SeptimaPlayer adept)
        {
            return BaseBasicAttackDamage;
        }

        public virtual float GetTagSkillPower(Player player, SeptimaPlayer adept, Tag tag, int tagCount)
        {
            return BaseBasicAttackDamage;
        }

        public virtual float GetSecondarySkillPower(Player player, SeptimaPlayer adept)
        {
            return BaseSecondaryAttackDamage;
        }

        // Essentially a duplicate of Player.ApplyDamageToNPC, but returns the final damage
        public static int ApplyDamageToNPCAndReturnFinalDamage(Player player, NPC npc, int damage, 
            float knockback, int direction, bool crit = false, DamageClass? damageType = null, 
            bool damageVariation = false)
        {
            if (!PlayerLoader.CanHitNPC(player, npc))
                return 0;

            var modifiers = npc.GetIncomingStrikeModifiers(damageType ?? DamageClass.Default, 0);
            PlayerLoader.ModifyHitNPC(player, npc, ref modifiers);

            player.ApplyBannerOffenseBuff(npc, ref modifiers);

            modifiers.ArmorPenetration += player.GetTotalArmorPenetration(damageType ?? DamageClass.Generic);

            player.OnHit(npc.Center.X, npc.Center.Y, npc);
            
            NPCKillAttempt attempt = new NPCKillAttempt(npc);
            NPC.HitInfo hit = modifiers.ToHitInfo(damage, crit, knockback, damageVariation, player.luck);
            int dmg = npc.StrikeNPC(hit);
            PlayerLoader.OnHitNPC(player, npc, hit, dmg);

            if (player.accDreamCatcher && !npc.HideStrikeDamage)
                player.addDPS(dmg);

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendStrikeNPC(npc, hit);

            int num2 = Item.NPCtoBanner(npc.BannerID());
            if (num2 >= 0)
                player.lastCreatureHit = num2;

            if (attempt.DidNPCDie())
                player.OnKillNPC(ref attempt, null);

            return dmg;
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
